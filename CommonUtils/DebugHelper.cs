using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CommonUtils
{
    public static class DebugHelper
    {
        private static string logHeader = "[KFantasmic] ";
        /// <summary>
        /// This method will write error log and show error dialog to the user.
        /// </summary>
        /// <param name="ex">Exception instance.</param>
        /// <param name="message">Error message to show.</param>
        /// <returns></returns>
        public static async Task ShowErrorDialog(Exception ex, string message)
        {
            WriteErrorLog(ex, message);
            var dialog = new ContentDialog
            {
                Title = "Error occured",
                Content = message + " : [" + ex.GetType().FullName + "] " + ex.Message,
                PrimaryButtonText = "OK",
            };
            await dialog.ShowAsync();
        }

        public static void WriteErrorLog(Exception ex, string message)
        {
            WriteDebugLog(message + " : [" + ex.GetType().FullName + "] " + ex.Message);
            WriteDebugLog(ex.ToString());
            if (ex.InnerException != null)
            {
                WriteErrorLog(ex.InnerException, "Inner Exception: ");
            }
        }

        public static void WriteDebugLog(string message)
        {
            Debug.WriteLine(logHeader + message);
        }
    }
}
