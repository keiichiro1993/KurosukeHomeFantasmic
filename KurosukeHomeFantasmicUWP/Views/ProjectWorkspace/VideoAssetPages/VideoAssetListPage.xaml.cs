using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace;
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

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.VideoAssetPages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class VideoAssetListPage : Page
    {
        public VideoAssetListPageViewModel ViewModel { get; set; } = new VideoAssetListPageViewModel();
        public VideoAssetListPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ViewModel.Init();
        }

        private async void AddVideoButton_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await ViewModel.AddVideo();
            ((Button)sender).IsEnabled = true;
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Frame.Navigate(typeof(VideoAssetPages.VideoPlaybackPage), ViewModel.SelectedVideo);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(VideoAssetPages.VideoPlaybackPage), e.ClickedItem);
        }
    }
}
