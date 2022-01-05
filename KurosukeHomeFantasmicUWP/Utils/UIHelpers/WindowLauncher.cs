using KurosukeHomeFantasmicUWP.Views.ProjectWorkspace;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KurosukeHomeFantasmicUWP.Utils.UIHelpers
{
    public static class WindowLauncher
    {
        public static void MoveToWorkspaceWindow(float width = 1920, float height = 1080)
        {
            float DPI = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi;
            var desiredSize = new Size((width * 96.0f / DPI), (height * 96.0f / DPI));
            ApplicationView.GetForCurrentView().TryResizeView(desiredSize);

            var frame = Window.Current.Content as Frame;
            if (frame != null)
            {
                frame.Navigate(typeof(ProjectWorkspaceWindow));
            }
        }
    }
}
