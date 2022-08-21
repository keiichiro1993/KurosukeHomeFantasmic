using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
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
    internal sealed partial class VideoTimelinePreviewChildControl : TimelinePlayerBase
    {
        public VideoTimelinePreviewChildControl()
        {
            this.InitializeComponent();
        }

        public override void UpdatePlaybackState()
        {
            if (playerElement.MediaPlayer.PlaybackSession.PlaybackState != PlaybackState)
            {
                switch (PlaybackState)
                {
                    case MediaPlaybackState.Playing:
                        playerElement.MediaPlayer.Play();
                        break;
                    case MediaPlaybackState.Paused:
                        playerElement.MediaPlayer.Pause();
                        break;
                }
            }
        }

        private TimelineVideoItem videoItem;
        public override void UpdatePosition()
        {
            if (Timeline != null && CurrentPosition != null)
            {
                var candidates = from item in base.Timeline.TimelineItems
                                 where ((TimelineVideoItem)item).StartTime <= CurrentPosition && ((TimelineVideoItem)item).EndTime >= CurrentPosition
                                 select item;
                if (candidates.Any())
                {
                    // choose the item with latest StartTime
                    var targetItem = (from item in candidates
                                      orderby ((TimelineVideoItem)item).StartTime descending
                                      select item).First() as TimelineVideoItem;
                    if (videoItem == null || videoItem.VideoAsset.VideoAssetEntity.Id != targetItem.VideoAsset.VideoAssetEntity.Id)
                    {
                        videoItem = targetItem;
                    }

                    if (videoItem != null && playerElement.MediaPlayer.Source == null)
                    {
                        playerElement.MediaPlayer.Source = videoItem.VideoMediaSource;
                    }

                    var videoPosition = CurrentPosition - targetItem.StartTime;

                    if ((playerElement.MediaPlayer.PlaybackSession.Position - videoPosition).Duration() > TimeSpan.FromMilliseconds(150))
                    {
                        playerElement.MediaPlayer.PlaybackSession.Position = videoPosition;
                    }
                }
                else
                {
                    videoItem = null;
                    playerElement.MediaPlayer.Source = null;
                }
            }
        }
    }
}
