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
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace Csharp5
{
    /// <summary>
    /// Interaction logic for DEMO_INSIDE_AWAIT.xaml
    /// </summary>
    public partial class DEMO_INSIDE_AWAIT : Window
    {
        public DEMO_INSIDE_AWAIT()
        {
            InitializeComponent();
        }

        private void doSomethingButton_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Calling DoWork");
			DoWork();
			Debug.WriteLine("DoWork Returned");
		}
		
		private async void DoWork()
		{
			string result = await CustomAsync.DoSomethingAsync();
			resultTextBlock.Text = result;
		}

        #region RETURNING TASKS: 
	    private async Task<string> GetHeaders()
	    {
		    var req = (HttpWebRequest) WebRequest.Create("http://www.pluralsight.com/");
		    req.Method = "HEAD";
		    var getResponseTask = Task.Factory.FromAsync<WebResponse>(
			    req.BeginGetResponse, req.EndGetResponse, null);
			    var resp = (HttpWebResponse) await getResponseTask;
			
			    string result = FormatHeaders(resp.Headers);
			    return result;
	    }
        #endregion

        private string FormatHeaders(WebHeaderCollection headers)
        {
            var headerStrings = from header in headers.Keys.Cast<string>()
                                select string.Format("{0}: {1}", header, headers[header]);

            return string.Join(Environment.NewLine, headerStrings.ToArray());
        }

        //http://services.mtps.microsoft.com/ServiceAPI/
    }

    class CustomAsync
	{
		public static CustomAwaitable DoSomethingAsync()
		{
			return new CustomAwaitable();
		}
	}
	class CustomAwaitable
	{
		public CustomAwaiter GetAwaiter()
		{
			return new CustomAwaiter();
		}
	}
	class CustomAwaiter
	{
		public bool BeginAwait(Action callback)
		{
			var ctx = SynchronizationContext.Current;
			var t = new Timer(delegate
			{
				ctx.Post(
					delegate
					{
						callback();
					}, null);
			}, null,
			5000, 
			-1);
			return false; //Needs to be true for the timer to work
		}
		public string EndAwait()
		{
			return "";
		}
	}
}
