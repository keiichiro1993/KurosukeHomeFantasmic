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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.AssetPages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AssetParentPage : Page
    {
        public AssetParentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            navigation.SelectedItem = navigation.MenuItems[0];
            contentFrame.Navigate(typeof(VideoAssetListPage));
        }

        private void navigation_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            Type pageType = null;
            switch (args.InvokedItem)
            {
                case "Video":
                    pageType = typeof(VideoAssetListPage);
                    break;
                case "Hue Action":
                    pageType = typeof(HueActionListPage);
                    break;
                case "Hue Effect":
                    pageType = typeof(HueEffectListPage);
                    break;
                case "Video (Remote Device)":
                    pageType = typeof(RemoteVideoAssetListPage);
                    break;
            }
            if (pageType != null)
            {
                contentFrame.NavigateToType(pageType, null, navOptions);
            }
        }

    }
}
