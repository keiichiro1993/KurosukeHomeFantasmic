using CommonUtils;
using KurosukeHomeFantasmicUWP.Controls.Players;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using KurosukeHueClient.Utils;
using KurosukeHomeFantasmicUWP.Utils;
using System.Threading.Tasks;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Players
{
    internal sealed partial class HueTimelinePlayerChild : TimelinePlayerBase
    {
        public HueTimelinePlayerChild()
        {
            this.InitializeComponent();
        }

        private TimelineHueItem hueItem;

        public override async void UpdatePlaybackState()
        {
            // Skip if null
            if (PlaybackState == null)
            {
                return;
            }

            if (PlaybackState == MediaPlaybackState.Paused)
            {
                lock (AppGlobalVariables.HueClientLock)
                {
                    if (AppGlobalVariables.GlobalHueClient != null)
                    {
                        //HueClient.Dispose() will cancel all operations
                        AppGlobalVariables.GlobalHueClient.Dispose();
                        AppGlobalVariables.GlobalHueClient = null;
                    }
                }

                if (hueItem != null)
                {
                    hueItem = null;
                }
            }
            else if (PlaybackState == MediaPlaybackState.Playing)
            {
                var needInit = false;

                lock (AppGlobalVariables.HueClientLock)
                {
                    if (AppGlobalVariables.GlobalHueClient == null)
                    {
                        var user = Utils.RequestHelpers.HueRequestHelper.GetHueUser();
                        AppGlobalVariables.GlobalHueClient = new HueClient(user);
                        needInit = true;
                    }
                }

                if (needInit)
                {
                    var groups = await AppGlobalVariables.GlobalHueClient.GetEntertainmentGroupsAsync();
                    var group = Utils.RequestHelpers.HueRequestHelper.GetHueGroup(groups);
                    await AppGlobalVariables.GlobalHueClient.ConnectEntertainmentGroup(group);
                }
            }
        }

        public override async void UpdatePosition()
        {
            // Skip if null
            if (Timeline == null || CurrentPosition == null)
            {
                return;
            }

            var candidates = from TimelineHueItem item in Timeline.TimelineItems
                             where item.StartTime <= CurrentPosition && item.EndTime >= CurrentPosition
                             orderby item.StartTime descending // choose the item with latest StartTime
                             select item;
            if (candidates.Any())
            {
                var targetItem = candidates.First();

                // item has been triggered in the last loop
                if (hueItem != null && hueItem.HueItemType == targetItem.HueItemType && hueItem.ItemId == targetItem.ItemId)
                {
                    return;
                }

                if (PlaybackState == MediaPlaybackState.Playing)
                {
                    // wait for HueClient to be connected
                    if (AppGlobalVariables.GlobalHueClient == null || !AppGlobalVariables.GlobalHueClient.IsConnected)
                    {
                        return;
                    }

                    // trigger hue API
                    DebugHelper.WriteDebugLog($"Trigger Hue API: {targetItem.HueItemType} - {targetItem.Name}({targetItem.ItemId})");
                    hueItem = targetItem;
                    try
                    {
                        if (hueItem.HueItemType == TimelineHueItem.TimelineHueItemTypes.Action)
                        {
                            AppGlobalVariables.GlobalHueClient.SendEntertainmentAction(hueItem.HueAction);
                        }
                        else
                        {
                            _ = AppGlobalVariables.GlobalHueClient.SendEntertainmentEffect(hueItem.HueEffect, hueItem.Duration);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        //TODO: await 無し非同期なのでタスクそれぞれ確認しないと catch 不能なんだが？
                        // https://qiita.com/tera1707/items/d5a3bc12ffa5f80069a1
                        // エラー出力コンソールみたいの実装してそこに流す案に一票
                    }
                    catch (Exception ex) 
                    {
                        OnMemoryCache.GlobalViewModel.GlobalPlaybackState = MediaPlaybackState.Paused;
                        await DebugHelper.ShowErrorDialog(ex, "Error in Hue Player");
                    }
                }
            }
            else
            {
                hueItem = null;
            }
        }
    }
}
