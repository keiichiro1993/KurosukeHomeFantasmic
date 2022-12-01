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
    public sealed partial class TimelineHueItemControl : TimelineItemBase, ITimelineItemControl
    {
        public TimelineHueItemControl()
        {
            this.InitializeComponent();
        }

        public override ITimelineItem TimelineItem
        {
            get => (TimelineHueItem)GetValue(HueItemProperty);
            set => SetValue(HueItemProperty, value);
        }

        public static readonly DependencyProperty HueItemProperty =
          DependencyProperty.Register(nameof(TimelineItem), typeof(TimelineHueItem), typeof(TimelineHueItemControl),
              new PropertyMetadata(null, HueItemChanged));

        private static void HueItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var instance = (TimelineHueItemControl)d;
            //instance.HueItem = e.NewValue as TimelineHueItem;
        }

        public TimelineHueItem TimelineHueItem { get { return (TimelineHueItem)TimelineItem; } }
    }
}
