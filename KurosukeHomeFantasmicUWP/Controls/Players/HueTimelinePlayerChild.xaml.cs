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
            if (PlaybackState != null)
            {
                switch (PlaybackState)
                {
                    case MediaPlaybackState.Paused:
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
                        break;
                    case MediaPlaybackState.Playing:
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

                        break;
                }
            }
        }
        public override void UpdatePosition()
        {
            if (Timeline != null && CurrentPosition != null)
            {
                var candidates = from item in Timeline.TimelineItems
                                 where ((TimelineHueItem)item).StartTime <= CurrentPosition && ((TimelineHueItem)item).EndTime >= CurrentPosition
                                 select item;
                if (candidates.Any())
                {
                    // choose the item with latest StartTime
                    var targetItem = (from item in candidates
                                      orderby ((TimelineHueItem)item).StartTime descending
                                      select item).First() as TimelineHueItem;
                    if (hueItem == null || hueItem.HueItemType != targetItem.HueItemType || hueItem.ItemId != targetItem.ItemId)
                    {
                        if (PlaybackState == MediaPlaybackState.Playing)
                        {
                            // wait for HueClient to be initialized in case of resume
                            if (AppGlobalVariables.GlobalHueClient != null && AppGlobalVariables.GlobalHueClient.IsConnected)
                            {
                                hueItem = targetItem;
                                if (hueItem.HueItemType == TimelineHueItem.TimelineHueItemTypes.Action)
                                {
                                    // trigger hue API
                                    DebugHelper.WriteDebugLog($"Trigger Hue API: {targetItem.HueItemType} - {targetItem.Name}({targetItem.ItemId})");
                                    AppGlobalVariables.GlobalHueClient.SendEntertainmentAction(hueItem.HueAction);
                                }
                            }
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
}
