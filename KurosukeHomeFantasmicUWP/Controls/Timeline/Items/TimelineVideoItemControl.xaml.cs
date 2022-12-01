using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Timeline.Items
{
    public sealed partial class TimelineVideoItemControl : TimelineItemBase, ITimelineItemControl
    {
        public TimelineVideoItemControl()
        {
            this.InitializeComponent();
        }

        public override ITimelineItem TimelineItem
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

        public TimelineVideoItem TimelineVideoItem { get { return (TimelineVideoItem)TimelineItem; } }

        // Manipulations
        private new void ResizeStartButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                ((TimelineVideoItem)TimelineItem).VideoStartPosition += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        private new void ResizeEndButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                ((TimelineVideoItem)TimelineItem).VideoEndPosition += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }
    }
}
