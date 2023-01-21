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
    public sealed partial class RemoteVideoAssetListPage : Page
    {
        public RemoteVideoAssetListPageViewModel ViewModel { get; set; } = new RemoteVideoAssetListPageViewModel();
        public RemoteVideoAssetListPage()
        {
            this.InitializeComponent();
        }

        private async void AddRemoteVideoButton_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            var dialog = new Controls.ContentDialogs.AddRemoteVideoAssetDialog();
            await dialog.ShowAsync();
            ((Button)sender).IsEnabled = true;
        }

        private async void RemoteVideoAssetListItem_DeleteButtonClicked(object sender, Utils.UIHelpers.ItemDeleteButtonClickedEventArgs<Models.RemoteVideoAsset> args)
        {
            var dialog = new MessageDialog($"Are you sure to delete Remote Video {args.DeleteItem.Info.Name} (Host: {args.DeleteItem.HostName})?", "Delete Remote Video");
            dialog.Commands.Add(new UICommand("Delete"));
            dialog.Commands.Add(new UICommand("Cancel"));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if (result.Label == "Delete")
            {
                foreach (var scene in OnMemoryCache.Scenes)
                {
                    var remoteVideoTimelines = from timeline in scene.Timelines
                                       where timeline.TimelineType == Timeline.TimelineTypes.RemoteVideo
                                       select timeline;
                    foreach (var timeline in remoteVideoTimelines)
                    {
                        var items = (from TimelineRemoteVideoItem item in timeline.TimelineItems
                                     where item.RemoteVideoAsset.DomainName == args.DeleteItem.DomainName && item.ItemId == args.DeleteItem.Id
                                     select item).ToList();
                        foreach (var item in items) { timeline.TimelineItems.Remove(item); }
                    }
                }
                OnMemoryCache.RemoteVideoAssets.Remove(args.DeleteItem);
            }
        }
    }
}
