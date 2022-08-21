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
    internal sealed partial class HueTimelinePlayerChild : UserControl
    {
        public HueTimelinePlayerChild()
        {
            this.InitializeComponent();
        }

        public TimeSpan CurrentPosition
        {
            get => (TimeSpan)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public static readonly DependencyProperty CurrentPositionProperty =
          DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(HueTimelinePlayerChild),
              new PropertyMetadata(null, CurrentPositionChanged));

        private static void CurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (HueTimelinePlayerChild)d;
            playerControl.UpdatePosition();
            playerControl.UpdatePlaybackState();
        }

        public MediaPlaybackState? PlaybackState
        {
            get => (MediaPlaybackState?)GetValue(PlaybackStateProperty);
            set => SetValue(PlaybackStateProperty, value);
        }

        public static readonly DependencyProperty PlaybackStateProperty =
            DependencyProperty.Register(nameof(PlaybackState), typeof(MediaPlaybackState?), typeof(HueTimelinePlayerChild),
                new PropertyMetadata(null, PlaybackStateChanged));

        private static void PlaybackStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (HueTimelinePlayerChild)d;
            playerControl.UpdatePlaybackState();
        }

        public Models.Timeline.Timeline HueTimeline
        {
            get => (Models.Timeline.Timeline)GetValue(HueTimelineProperty);
            set => SetValue(HueTimelineProperty, value);
        }

        public static readonly DependencyProperty HueTimelineProperty =
            DependencyProperty.Register(nameof(HueTimeline), typeof(Models.Timeline.Timeline), typeof(HueTimelinePlayerChild),
                new PropertyMetadata(null, VideoTimelineChanged));

        private static void VideoTimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (HueTimelinePlayerChild)d;
            playerControl.UpdatePosition();
            playerControl.UpdatePlaybackState();
        }

        private CancellationTokenSource cancellationTokenSource;
        private TimelineHueItem hueItem;

        public void UpdatePlaybackState()
        {
            if (PlaybackState != null && PlaybackState == MediaPlaybackState.Paused)
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
        }
        public void UpdatePosition()
        {
            if (HueTimeline != null && CurrentPosition != null)
            {
                var candidates = from item in HueTimeline.TimelineItems
                                 where ((TimelineVideoItem)item).StartTime <= CurrentPosition && ((TimelineVideoItem)item).EndTime >= CurrentPosition
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
