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
    public sealed partial class TimelineHueItemControl : UserControl, ITimelineItemControl
    {
        public TimelineHueItemControl()
        {
            this.InitializeComponent();
        }

        public ITimelineItem TimelineItem
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

        public delegate void DeleteButtonClickedEventHandler(object sender, ItemDeleteButtonClickedEventArgs<ITimelineItem> args);
        public event DeleteButtonClickedEventHandler DeleteButtonCliecked;

        public TimelineHueItem TimelineHueItem { get { return (TimelineHueItem)TimelineItem; } }

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
                TimelineItem.StartTime += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        private void ResizeEndButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                TimelineItem.Duration += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        private void ContextMenuDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteButtonCliecked != null)
            {
                this.DeleteButtonCliecked(sender, new ItemDeleteButtonClickedEventArgs<ITimelineItem>(this.TimelineItem));
            }
        }
    }
}
