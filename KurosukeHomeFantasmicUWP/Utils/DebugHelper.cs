using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace KurosukeHomeFantasmicUWP.Utils
{
    public static class DebugHelper
    {
        public static async Task ShowErrorDialog(Exception ex, string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Error occured",
                Content = message + " : [" + ex.GetType().FullName + "] " + ex.Message,
                PrimaryButtonText = "OK",
            };
            await dialog.ShowAsync();
        }
    }
}
