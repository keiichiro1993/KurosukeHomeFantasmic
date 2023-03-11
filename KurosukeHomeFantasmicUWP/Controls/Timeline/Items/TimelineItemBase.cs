using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KurosukeHomeFantasmicUWP.Controls.Timeline.Items
{
    public abstract class TimelineItemBase : UserControl, ITimelineItemControl
    {
        public abstract ITimelineItem TimelineItem { get; set; }
        public event DeleteButtonClickedEventHandler<ITimelineItem> DeleteButtonClicked;

        // Manipulations
        protected void UserControl_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                TimelineItem.StartTime += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        protected void UserControl_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            this.Opacity = 0.7;
        }

        protected void UserControl_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            this.Opacity = 1;
        }

        protected void ResizeButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 10);
            }
        }

        protected void ResizeButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
            }
        }

        protected void ResizeStartButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                TimelineItem.StartTime += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
                TimelineItem.Duration -= TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        protected void ResizeEndButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!TimelineItem.Locked)
            {
                var x = e.Delta.Translation.X;
                TimelineItem.Duration += TimelineItem.TotalCanvasDuration * (x / TimelineItem.CanvasWidth);
            }
        }

        protected void ContextMenuDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteButtonClicked != null)
            {
                this.DeleteButtonClicked(this, new ItemDeleteButtonClickedEventArgs<ITimelineItem>(TimelineItem));
            }
        }
    }
}
