using CommonUtils;
using KurosukeHomeFantasmicUWP.Controls.Timeline.Items;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
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
                AddItemControl(item);
            }
            TimelineData.TimelineItems.CollectionChanged += TimelineItems_CollectionChanged;
        }

        private void TimelineItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var newItem in e.NewItems.Cast<ITimelineItem>())
                {
                    AddItemControl(newItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<ITimelineItem>())
                {
                    var removeElement = (from child in singleTimeline.Children
                                         where (child as ITimelineItemControl) != null && ((ITimelineItemControl)child).TimelineItem == item
                                         select child).FirstOrDefault();
                    if (removeElement != null) { singleTimeline.Children.Remove(removeElement); }
                }
            }
        }

        private void AddItemControl(ITimelineItem newItem)
        {
            ITimelineItemControl control = null;
            switch (TimelineData.TimelineType)
            {
                // Video Items
                case Models.Timeline.Timeline.TimelineTypes.Video:
                    control = new TimelineVideoItemControl();
                    break;
                // Hue Items
                case Models.Timeline.Timeline.TimelineTypes.Hue:
                    control = new TimelineHueItemControl();
                    break;
                // Remote Video Items
                case Models.Timeline.Timeline.TimelineTypes.RemoteVideo:
                    control = new TimelineRemoteVideoItemControl();
                    break;
            }

            if (control != null)
            {
                control.TimelineItem = newItem;
                control.DeleteButtonClicked += TimelineItem_DeleteButtonCliecked;
                singleTimeline.Children.Add((UIElement)control);
            }
        }

        private void TimelineItem_DeleteButtonCliecked(object sender, Utils.UIHelpers.ItemDeleteButtonClickedEventArgs<ITimelineItem> args)
        {
            singleTimeline.Children.Remove((UIElement)sender);
            TimelineData.TimelineItems.Remove(args.DeleteItem);
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
            else if (e.DataView.Properties.TryGetValue("RemoteVideoAsset", out outObject))
            {
                var item = outObject as Models.RemoteVideoAsset;
                if (item != null)
                {
                    try
                    {
                        var videoItem = new TimelineRemoteVideoItem();
                        await videoItem.Init(item);
                        videoItem.CanvasWidth = singleTimelineBackground.ActualWidth;
                        videoItem.TotalCanvasDuration = TotalCanvasDuration;
                        TimelineData.TimelineItems.Add(videoItem);
                        if (string.IsNullOrEmpty(TimelineData.TargetDomainName))
                        {
                            TimelineData.TargetDomainName = item.DomainName;
                        }
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
                var item = outObject as HueEffect;
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
                        await DebugHelper.ShowErrorDialog(ex, "Failed to add the Hue Effect to the timeline.");
                    }
                }
            }
        }

        private void singleTimeline_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsContentVisible = true;
            object outObject = null;
            e.AcceptedOperation = DataPackageOperation.None;
            e.DataView.Properties.TryGetValue("TimelineType", out outObject);
            if (outObject == null || (string)outObject != TimelineData.TimelineType.ToString())
            {
                return;
            }

            if ((string)outObject == "RemoteVideo" && !string.IsNullOrEmpty(TimelineData.TargetDomainName))
            {
                e.DataView.Properties.TryGetValue("DomainName", out outObject);
                if ((string)outObject != TimelineData.TargetDomainName)
                {
                    return;
                }
            }

            e.AcceptedOperation = DataPackageOperation.Copy;
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

        public event DeleteButtonClickedEventHandler<Models.Timeline.Timeline> DeleteButtonClicked;
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteButtonClicked != null)
            {
                this.DeleteButtonClicked(this, new ItemDeleteButtonClickedEventArgs<Models.Timeline.Timeline>(TimelineData));
            }
        }
    }

    public class SingleTimelineViewModel : ViewModelBase
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
