using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Timeline.Items
{
    public sealed partial class TimelineVideoItemControl : UserControl, ITimelineItemControl
    {
        public TimelineVideoItemControl()
        {
            this.InitializeComponent();
        }

        public ITimelineItem TimelineItem
        {
            get => (TimelineVideoItem)GetValue(VideoItemProperty);
            set => SetValue(VideoItemProperty, value);
        }

        public static readonly DependencyProperty VideoItemProperty =
          DependencyProperty.Register(nameof(TimelineItem), typeof(TimelineVideoItem), typeof(TimelineVideoItemControl),
              new PropertyMetadata(null, VideoItemChanged));

        private static void VideoItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var instance = (TimelineVideoItemControl)d;
            //instance.VideoItem = e.NewValue as TimelineVideoItem;
        }

        public delegate void DeleteButtonClickedEventHandler(object sender, ItemDeleteButtonClickedEventArgs<ITimelineItem> args);
        public event DeleteButtonClickedEventHandler DeleteButtonClicked;

        public TimelineVideoItem TimelineVideoItem { get { return (TimelineVideoItem)TimelineItem; } }

        // Manipulations
        private void UserControl_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                TimelineItem.StartTime += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
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
            if (!TimelineItem.Locked)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 10);
            }
        }

        private void ResizeButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
            }
        }

        private void ResizeStartButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                ((TimelineVideoItem)TimelineItem).VideoStartPosition += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        private void ResizeEndButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                ((TimelineVideoItem)TimelineItem).VideoEndPosition += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        private void ContextMenuDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteButtonClicked != null)
            {
                this.DeleteButtonClicked(this, new ItemDeleteButtonClickedEventArgs<ITimelineItem>(this.TimelineItem));
            }
        }
    }
}
