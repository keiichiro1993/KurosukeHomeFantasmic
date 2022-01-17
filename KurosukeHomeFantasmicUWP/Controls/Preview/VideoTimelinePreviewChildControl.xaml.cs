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

namespace KurosukeHomeFantasmicUWP.Controls.Preview
{
    public sealed partial class VideoTimelinePreviewChildControl : UserControl
    {
        VideoTimelinePreviewChildControlViewModel ViewModel { get; set; } = new VideoTimelinePreviewChildControlViewModel();
        public VideoTimelinePreviewChildControl()
        {
            this.InitializeComponent();
            this.Loaded += VideoTimelinePreviewControl_Loaded;
        }

        private void VideoTimelinePreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            playerElement.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            // There must be a pattern that the playback state of other preview is still Playing but this preview doesn't have anything to play.
            // have to think the way to sync
            //PlaybackState = sender.PlaybackState;
        }

        public TimeSpan CurrentPosition
        {
            get => (TimeSpan)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public static readonly DependencyProperty CurrentPositionProperty =
          DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(VideoTimelinePreviewChildControl),
              new PropertyMetadata(null, CurrentPositionChanged));

        private static async void CurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var previewControl = (VideoTimelinePreviewChildControl)d;
            await previewControl.UpdatePosition();
            previewControl.UpdatePlaybackState();
        }

        public MediaPlaybackState PlaybackState
        {
            get => (MediaPlaybackState)GetValue(PlaybackStateProperty);
            set => SetValue(PlaybackStateProperty, value);
        }

        public static readonly DependencyProperty PlaybackStateProperty =
            DependencyProperty.Register(nameof(PlaybackState), typeof(MediaPlaybackState), typeof(VideoTimelinePreviewChildControl),
                new PropertyMetadata(null, PlaybackStateChanged));

        private static void PlaybackStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var previewControl = (VideoTimelinePreviewChildControl)d;
            previewControl.UpdatePlaybackState();
        }

        public void UpdatePlaybackState()
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

        public ITimeline VideoTimeline
        {
            get => (ITimeline)GetValue(VideoTimelineProperty);
            set => SetValue(VideoTimelineProperty, value);
        }

        public static readonly DependencyProperty VideoTimelineProperty =
            DependencyProperty.Register(nameof(VideoTimeline), typeof(ITimeline), typeof(VideoTimelinePreviewChildControl),
                new PropertyMetadata(null, VideoTimelineChanged));

        private static async void VideoTimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var previewControl = (VideoTimelinePreviewChildControl)d;
            await previewControl.UpdatePosition();
            previewControl.UpdatePlaybackState();
        }

        private Models.VideoAsset videoAsset;
        public async Task UpdatePosition()
        {
            if (VideoTimeline != null && CurrentPosition != null)
            {
                var candidates = from item in VideoTimeline.TimelineItems
                                 where ((TimelineVideoItem)item).StartTime <= CurrentPosition && ((TimelineVideoItem)item).EndTime >= CurrentPosition
                                 select item;
                if (candidates.Any())
                {
                    // choose the item with latest StartTime
                    var targetItem = (from item in candidates
                                      orderby ((TimelineVideoItem)item).StartTime descending
                                      select item).First() as TimelineVideoItem;
                    if (videoAsset == null || videoAsset.VideoAssetEntity.Id != targetItem.VideoAsset.VideoAssetEntity.Id)
                    {
                        playerElement.MediaPlayer.Source = MediaSource.CreateFromStorageFile(await targetItem.VideoAsset.GetVideoAssetFile());
                        videoAsset = targetItem.VideoAsset;
                    }

                    var videoPosition = CurrentPosition - targetItem.StartTime;

                    if ((playerElement.MediaPlayer.PlaybackSession.Position - videoPosition).Duration() > TimeSpan.FromMilliseconds(150))
                    {
                        playerElement.MediaPlayer.PlaybackSession.Position = videoPosition;
                    }
                }
                else
                {
                    videoAsset = null;
                    playerElement.MediaPlayer.Source = null;
                }
            }
        }
    }

    public class VideoTimelinePreviewChildControlViewModel : ViewModels.ViewModelBase
    {

    }
}
