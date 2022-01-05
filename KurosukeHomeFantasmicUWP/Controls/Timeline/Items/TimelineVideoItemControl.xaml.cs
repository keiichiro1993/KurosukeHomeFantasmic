using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Timeline.Items
{
    public sealed partial class TimelineVideoItemControl : UserControl
    {
        public TimelineVideoItemControl()
        {
            this.InitializeComponent();
        }

        public TimelineVideoItem VideoItem
        {
            get => (TimelineVideoItem)GetValue(VideoItemProperty);
            set => SetValue(VideoItemProperty, value);
        }

        public static readonly DependencyProperty VideoItemProperty =
          DependencyProperty.Register(nameof(VideoItem), typeof(TimelineVideoItem), typeof(TimelineVideoItemControl),
              new PropertyMetadata(null, VideoItemChanged));

        private static void VideoItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var instance = (TimelineVideoItemControl)d;
            //instance.VideoItem = e.NewValue as TimelineVideoItem;
        }

        // Manipulations
        private void UserControl_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!VideoItem.Locked)
            {
                var x = e.Delta.Translation.X;
                VideoItem.StartTime += VideoItem.TotalCanvasDuration * (x / VideoItem.CanvasWidth);
            }
        }

        private void UserControl_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            this.Opacity = 0.7;
        }

        private void UserControl_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            this.Opacity = 1;
        }

        private void ResizeButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!VideoItem.Locked)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 10);
            }
        }

        private void ResizeButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!VideoItem.Locked)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
            }
        }

        private void ResizeStartButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!VideoItem.Locked)
            {
                var x = e.Delta.Translation.X;
                VideoItem.VideoStartPosition += VideoItem.TotalCanvasDuration * (x / VideoItem.CanvasWidth);
            }
        }

        private void ResizeEndButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!VideoItem.Locked)
            {
                var x = e.Delta.Translation.X;
                VideoItem.VideoEndPosition += VideoItem.TotalCanvasDuration * (x / VideoItem.CanvasWidth);
            }
        }
    }
}
