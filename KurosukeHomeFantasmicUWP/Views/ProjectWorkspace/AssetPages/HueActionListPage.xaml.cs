using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.AssetPages;
using KurosukeHueClient.Models.HueObjects;
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
    public sealed partial class HueActionListPage : Page
    {
        public HueActionListPageViewModel ViewModel { get; set; } = new HueActionListPageViewModel();

        public HueActionListPage()
        {
            this.InitializeComponent();
        }

        private async void AddHueActionButton_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            var dialog = new Controls.ContentDialogs.AddHueActionDialog();
            await dialog.ShowAsync();
            ((Button)sender).IsEnabled = true;
        }

        public static HueAction HueAction(HueAction action) { return action; }

        private async void HueActionListItem_DeleteButtonClicked(object sender, Utils.UIHelpers.ItemDeleteButtonClickedEventArgs<HueAction> args)
        {
            var dialog = new MessageDialog(String.Format("Are you sure to delete action {0} (ID: {1})?", args.DeleteItem.Name, args.DeleteItem.Id), "Delete action");
            dialog.Commands.Add(new UICommand("Delete"));
            dialog.Commands.Add(new UICommand("Cancel"));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if (result.Label == "Delete")
            {
                foreach (var scene in OnMemoryCache.Scenes)
                {
                    var hueTimelines = from timeline in scene.Timelines
                                       where timeline.TimelineType == Timeline.TimelineTypes.Hue
                                       select timeline;
                    foreach (var timeline in hueTimelines)
                    {
                        var items = (from TimelineHueItem item in timeline.TimelineItems
                                    where item.HueItemType == TimelineHueItem.TimelineHueItemTypes.Action && item.ItemId == args.DeleteItem.Id
                                    select item).ToList();
                        foreach (var item in items) { timeline.TimelineItems.Remove(item); }
                    }
                }
                OnMemoryCache.HueActions.Remove(args.DeleteItem);
            }
        }
    }
}
