using CommonUtils;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KurosukeHomeFantasmicUWP.Controls.Players
{
    public abstract partial class TimelinePlayerBase : UserControl
    {
        public delegate void PropertyChangedDelegate<DependencyObject, DependencyPropertyChangedEventArgs>(DependencyObject d, DependencyPropertyChangedEventArgs e);
        public TimeSpan CurrentPosition
        {
            get => (TimeSpan)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public static readonly DependencyProperty CurrentPositionProperty =
          DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(TimelinePlayerBase),
              new PropertyMetadata(null, CurrentPositionChangedCallBack));

        public event PropertyChangedDelegate<DependencyObject, DependencyPropertyChangedEventArgs> CurrentPositionChanged;
        private static void CurrentPositionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (TimelinePlayerBase)d;
            if (playerControl.CurrentPositionChanged != null)
            {
                playerControl.CurrentPositionChanged(d, e);
            }

            playerControl.UpdatePosition();
            playerControl.UpdatePlaybackState();
        }

        public MediaPlaybackState? PlaybackState
        {
            get => (MediaPlaybackState?)GetValue(PlaybackStateProperty);
            set => SetValue(PlaybackStateProperty, value);
        }

        public static readonly DependencyProperty PlaybackStateProperty =
            DependencyProperty.Register(nameof(PlaybackState), typeof(MediaPlaybackState?), typeof(TimelinePlayerBase),
                new PropertyMetadata(null, PlaybackStateChangedCallBack));

        public event PropertyChangedDelegate<DependencyObject, DependencyPropertyChangedEventArgs> PlaybackStateChanged;
        private static void PlaybackStateChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (TimelinePlayerBase)d;
            if (playerControl.PlaybackStateChanged != null)
            {
                playerControl.PlaybackStateChanged(d, e);
            }

            playerControl.UpdatePlaybackState();
        }

        public Models.Timeline.Timeline Timeline
        {
            get => (Models.Timeline.Timeline)GetValue(TimelineProperty);
            set => SetValue(TimelineProperty, value);
        }

        public static readonly DependencyProperty TimelineProperty =
            DependencyProperty.Register(nameof(Timeline), typeof(Models.Timeline.Timeline), typeof(TimelinePlayerBase),
                new PropertyMetadata(null, TimelineChangedCallBack));

        public event PropertyChangedDelegate<DependencyObject, DependencyPropertyChangedEventArgs> TimelineChanged;

        private static void TimelineChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (TimelinePlayerBase)d;
            if (playerControl.TimelineChanged != null)
            {
                playerControl.TimelineChanged(d, e);
            }

            playerControl.UpdatePosition();
            playerControl.UpdatePlaybackState();
        }

        public abstract void UpdatePlaybackState();
        public abstract void UpdatePosition();
    }
}
