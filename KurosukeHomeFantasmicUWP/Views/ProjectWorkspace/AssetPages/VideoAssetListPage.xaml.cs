using KurosukeHomeFantasmicUWP.Controls.ContentDialogs;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
            Frame.Navigate(typeof(AssetPages.VideoPlaybackPage), e.ClickedItem);
        }

        private async void VideoAssetListItem_DeleteButtonClicked(object sender, Utils.UIHelpers.ItemDeleteButtonClickedEventArgs<Models.VideoAsset> args)
        {
            //TODO: move asset removal logic to content dialog
            var dialog = new MessageDialog(String.Format("Are you sure to delete video {0} (ID: {1})?", args.DeleteItem.VideoAssetEntity.Name, args.DeleteItem.VideoAssetEntity.Id), "Delete effect");
            dialog.Commands.Add(new UICommand("Delete"));
            dialog.Commands.Add(new UICommand("Cancel"));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if (result.Label == "Delete")
            {
                // remove from scenes
                foreach (var scene in OnMemoryCache.Scenes)
                {
                    var videoTimelines = from timeline in scene.Timelines
                                         where timeline.TimelineType == Timeline.TimelineTypes.Video
                                         select timeline;
                    foreach (var timeline in videoTimelines)
                    {
                        var items = (from TimelineVideoItem item in timeline.TimelineItems
                                     where item.ItemId == args.DeleteItem.VideoAssetEntity.Id
                                     select item).ToList();
                        foreach (var item in items) { timeline.TimelineItems.Remove(item); }
                    }
                }
                // invoke scene save to reflect change above, otherwise the project file may be broken after relaunch
                // since there's a removed video remaining on the timeline
                var saveDialog = new SaveDialog();
                await saveDialog.ShowAsync();

                // remove video file and from database
                await ViewModel.RemoveVideo(args.DeleteItem);
            }
        }
    }
}
