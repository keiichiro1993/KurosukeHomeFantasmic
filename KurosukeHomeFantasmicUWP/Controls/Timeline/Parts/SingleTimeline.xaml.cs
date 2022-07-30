using CommonUtils;
using KurosukeHomeFantasmicUWP.Controls.Timeline.Items;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHueClient.Models.HueObjects;
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
        SingleTimelineViewModel ViewModel { get; set; } = new SingleTimelineViewModel();
        public SingleTimeline()
        {
            this.InitializeComponent();
            this.Loaded += SingleTimeline_Loaded;
        }

        #region bindable properties
        public Models.Timeline.Timeline TimelineData
        {
            get => (Models.Timeline.Timeline)GetValue(TimelineDataProperty);
            set => SetValue(TimelineDataProperty, value);
        }

        public static readonly DependencyProperty TimelineDataProperty =
          DependencyProperty.Register(nameof(TimelineData), typeof(Models.Timeline.Timeline), typeof(SingleTimeline),
              new PropertyMetadata(null, TimelineDataChanged));

        private static void TimelineDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (SingleTimeline)d;
            instance.ViewModel.TimelineData = e.NewValue as Models.Timeline.Timeline;
        }

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

        public double ScrollViewerHorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
          DependencyProperty.Register(nameof(ScrollViewerHorizontalOffset), typeof(double), typeof(SingleTimeline),
              new PropertyMetadata(null, HorizontalOffsetChanged));

        private static void HorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (SingleTimeline)d;
            instance.ViewModel.ScrollViewerHorizontalOffset = (double)e.NewValue;
        }
        #endregion

        private void SingleTimeline_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in TimelineData.TimelineItems)
            {
                var control = new TimelineVideoItemControl();
                control.TimelineItem = (TimelineVideoItem)item;
                singleTimeline.Children.Add(control);
            }
            TimelineData.TimelineItems.CollectionChanged += TimelineItems_CollectionChanged;
        }

        private void TimelineItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var newItem in e.NewItems.Cast<ITimelineItem>())
                {
                    // Video Items
                    if (e.NewItems.GetType() == typeof(TimelineVideoItem))
                    {
                        var control = new TimelineVideoItemControl();
                        control.TimelineItem = newItem;
                        singleTimeline.Children.Add(control);
                    }
                    // Hue Items
                    else if (e.NewItems.GetType() == typeof(TimelineHueItem))
                    {
                        var control = new TimelineHueItemControl();
                        control.TimelineItem = newItem;
                        singleTimeline.Children.Add(control);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<ITimelineItem>())
                {
                    var removeElement = (from child in singleTimeline.Children
                                         where ((ITimelineItemControl)child).TimelineItem == item
                                         select child).FirstOrDefault();
                    if (removeElement != null) { singleTimeline.Children.Remove(removeElement); }
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
                    try
                    {
                        var videoItem = new TimelineVideoItem();
                        await videoItem.Init(item);
                        videoItem.CanvasWidth = singleTimelineBackground.ActualWidth;
                        videoItem.TotalCanvasDuration = TotalCanvasDuration;
                        TimelineData.TimelineItems.Add(videoItem);
                    }
                    catch (Exception ex)
                    {
                        await DebugHelper.ShowErrorDialog(ex, "Failed to add the Video to the timeline.");
                    }
                }
            }
            else if (e.DataView.Properties.TryGetValue("HueAction", out outObject))
            {
                var item = outObject as HueAction;
                if (item != null)
                {
                    try
                    {
                        var hueItem = new TimelineHueItem(item);
                        hueItem.CanvasWidth = singleTimelineBackground.ActualWidth;
                        hueItem.TotalCanvasDuration = TotalCanvasDuration;
                        TimelineData.TimelineItems.Add(hueItem);
                    }
                    catch (Exception ex)
                    {
                        await DebugHelper.ShowErrorDialog(ex, "Failed to add the Hue Action to the timeline.");
                    }
                }
            }
            else if (e.DataView.Properties.TryGetValue("HueEffect", out outObject))
            {
            }
        }

        private void singleTimeline_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsContentVisible = true;
            object outObject = null;
            e.DataView.Properties.TryGetValue("TimelineType", out outObject);
            if (outObject != null && (string)outObject == TimelineData.TimelineType.ToString())
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
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

    public class SingleTimelineViewModel : ViewModels.ViewModelBase
    {
        private Models.Timeline.Timeline _TimelineData;
        public Models.Timeline.Timeline TimelineData
        {
            get { return _TimelineData; }
            set
            {
                _TimelineData = value;
                RaisePropertyChanged();
            }
        }

        private double _ScrollViewerHorizontalOffset;
        public double ScrollViewerHorizontalOffset
        {
            get { return _ScrollViewerHorizontalOffset; }
            set
            {
                _ScrollViewerHorizontalOffset = value;
                RaisePropertyChanged();
            }
        }
    }
}
