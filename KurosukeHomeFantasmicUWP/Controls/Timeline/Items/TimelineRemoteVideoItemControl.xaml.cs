using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Timeline.Items
{
    public sealed partial class TimelineRemoteVideoItemControl : TimelineItemBase, ITimelineItemControl
    {
        public TimelineRemoteVideoItemControl()
        {
            this.InitializeComponent();
        }

        public override ITimelineItem TimelineItem
        {
            get => (TimelineRemoteVideoItem)GetValue(RemoteVideoItemProperty);
            set => SetValue(RemoteVideoItemProperty, value);
        }

        public static readonly DependencyProperty RemoteVideoItemProperty =
          DependencyProperty.Register(nameof(TimelineItem), typeof(TimelineRemoteVideoItem), typeof(TimelineRemoteVideoItemControl),
              new PropertyMetadata(null, RemoteVideoItemChanged));

        private static void RemoteVideoItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var instance = (TimelineRemoteVideoItemControl)d;
            //instance.HueItem = e.NewValue as TimelineHueItem;
        }

        public TimelineRemoteVideoItem TimelineRemoteVideoItem { get { return (TimelineRemoteVideoItem)TimelineItem; } }

        // Manipulations
        private new void ResizeStartButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                ((TimelineRemoteVideoItem)TimelineItem).VideoStartPosition += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        private new void ResizeEndButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                ((TimelineRemoteVideoItem)TimelineItem).VideoEndPosition += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }
    }
}
