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

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Players
{
    internal sealed partial class HueTimelinePlayerChild : TimelinePlayerBase
    {
        public HueTimelinePlayerChild()
        {
            this.InitializeComponent();
        }

        private CancellationTokenSource cancellationTokenSource;
        private TimelineHueItem hueItem;

        public override void UpdatePlaybackState()
        {
            if (base.PlaybackState != null && base.PlaybackState == MediaPlaybackState.Paused)
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
        }
        public override void UpdatePosition()
        {
            if (base.Timeline != null && base.CurrentPosition != null)
            {
                var candidates = from item in base.Timeline.TimelineItems
                                 where ((TimelineVideoItem)item).StartTime <= base.CurrentPosition && ((TimelineVideoItem)item).EndTime >= base.CurrentPosition
                                 select item;
                if (candidates.Any())
                {
                    // choose the item with latest StartTime
                    var targetItem = (from item in candidates
                                      orderby ((TimelineVideoItem)item).StartTime descending
                                      select item).First() as TimelineHueItem;
                    if (hueItem == null || hueItem.HueItemType != targetItem.HueItemType || hueItem.ItemId != targetItem.ItemId)
                    {
                        hueItem = targetItem;

                        // trigger hue API
                        DebugHelper.WriteDebugLog($"Trigger Hue API: {hueItem.HueItemType.ToString()} - {hueItem.Name}({hueItem.ItemId})");
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
