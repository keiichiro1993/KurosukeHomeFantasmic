using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Views.Settings
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SettingsParentPage : Page
    {
        public SettingsParentPage()
        {
            this.InitializeComponent();
        }

        private readonly List<(string Tag, Type Page)> pages = new List<(string Tag, Type Page)>
        {
            ("PanelLayout", typeof(PanelLayout))
        };

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type page = null;
            var item = pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            page = item.Page;
            var preNavPageType = contentFrame.CurrentSourcePageType;
            if (!(page is null) && !Type.Equals(preNavPageType, page))
            {
                contentFrame.Navigate(page, null, transitionInfo);
            }
        }

        private void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var navView = (Microsoft.UI.Xaml.Controls.NavigationView)sender;
            navView.SelectedItem = navView.MenuItems.First();
            NavView_Navigate("PanelLayout", null);
        }

        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
            else if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
