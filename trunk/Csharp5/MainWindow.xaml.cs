using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Csharp5
{
    //Asynchronous Programming in C# 5
    //You can get the beta version from this URL
    //http://msdn.microsoft.com/vstudio/async
    //WARNING this will override your existing compiler!!!
    //So its best to do this in a virtual machine

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            #region Synchronous CODE:
	        WebClient wc = new WebClient();
	        string txt = wc.DownloadString("http://www.pluralsight.com/");
	        dataTextBox.Text = txt;
            //program frezes until the webClient has finished.
            #endregion

            #region Asynchronous CODE:
            wc.DownloadStringAsync(new Uri("http://www.pluralsight.com/"));
            #endregion
        }

        void w_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
	    {
		    dataTextBox.Text = e.Result;
	    }

        #region Synchronous CODE:
        private void getButton_Click(object sender, RoutedEventArgs e)
        {
            var req = (HttpWebRequest)WebRequest.Create("http://www.pluralsight.com/");
            req.Method = "HEAD";
            var resp = (HttpWebResponse)req.GetResponse();
            string headersText = FormatHeaders(resp.Headers);
            dataTextBox.Text = headersText;
        }
        #endregion

        #region Asynchronous CODE:
        private void getButton_Click(object sender, RoutedEventArgs e)
        {
            var sync = SynchronizationContext.Current;
            var req = (HttpWebRequest)WebRequest.Create("http://www.pluralsight.com/");
            req.Method = "HEAD";
            req.BeginGetResponse(
                asyncResult =>
                {
                    var resp = (HttpWebResponse)req.EndGetResponse(asyncResult);
                    string headersText = FormatHeaders(resp.Headers);
                    sync.Post(
                        delegate
                        {
                            dataTextBox.Text = headersText;
                        },
                        null);
                },
                null);
        }
        #endregion

        private string FormatHeaders(WebHeaderCollection headers)
        {
            var headerStrings = from header in headers.Keys.Cast<string>()
                                select string.Format("{0}: {1}", header, headers[header]);

            return string.Join(Environment.NewLine, headerStrings.ToArray());
        }	

        #region  C# 5 Keywords:
        //async
        //await
	
        //example:
	    private async void get_Button_Click(object sender, RoutedEventArgs e)
	    {
		    WebClient wc = new WebClient();
		
		    string txt = await wc.DownloadStringTaskAsync("http://www.pluralsight.com/");
		    dataTextBox.Text = txt;
	    }
	
	    private async void getButton_Click(object sender, RoutedEventArgs e)
	    {
		    var req = (HttpWebRequest) WebRequest.Create("http://www.pluralsight.com/");
		    req.Method= "HEAD";
		    Task<WebResponse> getResponseTask = Task.Factory.FromAsync<WebResponse>(
			    req.BeginGetResponse, req.EndGetResponse, null);
		    var resp = (HttpWebResponse) await getResponseTask;
		    string headersText = FormatHeaders(resp.Headers);
		    dataTextBox.Text = headersText;
	    }
        #endregion
    }
}
