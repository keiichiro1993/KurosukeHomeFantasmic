using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.Settings
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SettingsMainPage : Page
    {
        public SettingsMainPage()
        {
            this.InitializeComponent();
        }

        private readonly List<(string Tag, Type Page)> pages = new List<(string Tag, Type Page)>
        {
            ("AccountSettings", typeof(AccountSettingsPage))
        };

        private void mainNavigation_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                //Frame.Navigate(typeof(Views.Settings.SettingsPage));
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                var page = pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));

                contentFrame.Navigate(page.Page);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var item = pages.First();
            mainNavigation.SelectedItem = mainNavigation.MenuItems.First();
            contentFrame.Navigate(item.Page);

            if (Frame.CanGoBack)
            {
                mainNavigation.IsBackEnabled = true;
                mainNavigation.BackRequested += MainNavigation_BackRequested;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            mainNavigation.BackRequested -= MainNavigation_BackRequested;
        }

        private void MainNavigation_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            Frame.GoBack();
        }
    }
}
