using KurosukeHomeFantasmicUWP.Controls.Timeline.Items;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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

namespace KurosukeHomeFantasmicUWP.Controls.Timeline
{
    public sealed partial class SingleTimeline : UserControl
    {
        public SingleTimeline()
        {
            this.InitializeComponent();
            this.Loaded += SingleTimeline_Loaded;
        }

        public ITimeline TimelineData
        {
            get => (ITimeline)GetValue(TimelineDataProperty);
            set => SetValue(TimelineDataProperty, value);
        }

        public static readonly DependencyProperty TimelineDataProperty =
          DependencyProperty.Register(nameof(TimelineData), typeof(ITimeline), typeof(SingleTimeline),
              new PropertyMetadata(null, null));

        public TimeSpan TotalCanvasDuration
        {
            get => (TimeSpan)GetValue(TotalCanvasDurationProperty);
            set => SetValue(TotalCanvasDurationProperty, value);
        }

        public static readonly DependencyProperty TotalCanvasDurationProperty =
          DependencyProperty.Register(nameof(TotalCanvasDuration), typeof(TimeSpan), typeof(SingleTimeline),
              new PropertyMetadata(null, TotalCanvasDurationChanged));

        private static void TotalCanvasDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeline = (SingleTimeline)d;
            foreach (var item in timeline.TimelineData.TimelineItems)
            {
                item.TotalCanvasDuration = (TimeSpan)e.NewValue;
            }
        }

        private void SingleTimeline_Loaded(object sender, RoutedEventArgs e)
        {
            TimelineData.TimelineItems.CollectionChanged += TimelineItems_CollectionChanged;
        }

        private void TimelineItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                var newItems = e.NewItems.Cast<TimelineVideoItem>();
                foreach (var newItem in newItems)
                {
                    var item = from childItem in singleTimeline.Children
                               where childItem.GetType() == typeof(TimelineVideoItemControl) && ((TimelineVideoItemControl)childItem).VideoItem == newItem
                               select childItem;
                    if (!item.Any())
                    {
                        var control = new TimelineVideoItemControl();
                        control.VideoItem = newItem;
                        singleTimeline.Children.Add(control);
                    }
                }
            }

            if (e.OldItems != null)
            {
                var oldItems = e.OldItems.Cast<TimelineVideoItem>();
                var removeItems = from childItem in singleTimeline.Children
                                  where childItem.GetType() == typeof(TimelineVideoItemControl) && oldItems.Contains(((TimelineVideoItemControl)childItem).VideoItem)
                                  select childItem;
                if (removeItems.Any())
                {
                    foreach (var element in removeItems) { singleTimeline.Children.Remove(element); }
                }
            }
        }

        private async void singleTimeline_Drop(object sender, DragEventArgs e)
        {
            object outObject = null;
            if (e.DataView.Properties.TryGetValue("VideoAsset", out outObject))
            {
                var item = outObject as Models.VideoAsset;
                if (item != null)
                {
                    var videoItem = new TimelineVideoItem();
                    await videoItem.Init(item);
                    videoItem.CanvasWidth = singleTimelineBackground.ActualWidth;
                    videoItem.TotalCanvasDuration = TotalCanvasDuration;
                    TimelineData.TimelineItems.Add(videoItem);
                }
            }
        }

        private void singleTimeline_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.IsContentVisible = true;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = sender as Grid;
            singleTimelineBackground.Width = grid.ActualWidth;
            singleTimelineBackground.Height = grid.ActualHeight;
            foreach (var item in TimelineData.TimelineItems)
            {
                item.CanvasWidth = grid.ActualWidth;
            }
        }
    }
}
