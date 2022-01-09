using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Timeline
{
    public sealed partial class TimelineControl : UserControl
    {
        public TimelineControlViewModel ViewModel { get; set; } = new TimelineControlViewModel();
        //public ObservableCollection<ITimeline> Timelines { get; set; } = new ObservableCollection<ITimeline>();
        public TimelineControl()
        {
            this.InitializeComponent();
            this.Loaded += TimelineControl_Loaded;
        }

        private void TimelineControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.TimelineWidth = rootGrid.ActualWidth;
        }

        public TimeSpan CurrentPosition
        {
            get => (TimeSpan)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public static readonly DependencyProperty CurrentPositionProperty =
          DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(TimelineControl),
              new PropertyMetadata(null, CurrentPositionChanged));

        private static void CurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timelineControl = (TimelineControl)d;
            timelineControl.ViewModel.CurrentPosition = (TimeSpan)e.NewValue;
        }

        public ObservableCollection<ITimeline> Timelines
        {
            get => (ObservableCollection<ITimeline>)GetValue(TimelinesProperty);
            set => SetValue(TimelinesProperty, value);
        }

        public static readonly DependencyProperty TimelinesProperty =
          DependencyProperty.Register(nameof(Timelines), typeof(ObservableCollection<ITimeline>), typeof(TimelineControl),
              new PropertyMetadata(null, TimelinesChanged));

        private static void TimelinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timelineControl = (TimelineControl)d;
            var newValue = (ObservableCollection<ITimeline>)e.NewValue;
            timelineControl.timelineParentPanel.Children.Clear();
            timelineControl.AddNewTimelineElements(newValue);
            newValue.CollectionChanged += timelineControl.Timelines_CollectionChanged;
        }

        private void Timelines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                AddNewTimelineElements(e.NewItems);
            }

            if (e.OldItems != null)
            {
                foreach (var removeItem in e.OldItems)
                {
                    var removeElement = from item in timelineParentPanel.Children
                                        where ((SingleTimeline)item).TimelineData.Id == ((ITimeline)removeItem).Id
                                        select item;
                    if (removeElement.Any())
                    {
                        foreach (var element in removeElement) { timelineParentPanel.Children.Remove(element); }
                    }
                }
            }
        }

        private void AddNewTimelineElements(System.Collections.IList newItems)
        {
            foreach (var item in newItems)
            {
                var timelineElement = new SingleTimeline
                {
                    TimelineData = (ITimeline)item,
                    TotalCanvasDuration = TotalCanvasDuration,
                    ScrollViewerHorizontalOffset = timelineScrollViewer.HorizontalOffset
                };
                timelineParentPanel.Children.Add(timelineElement);
            }
        }

        public TimeSpan TotalCanvasDuration
        {
            get => (TimeSpan)GetValue(TotalCanvasDurationProperty);
            set => SetValue(TotalCanvasDurationProperty, value);
        }

        public static readonly DependencyProperty TotalCanvasDurationProperty =
          DependencyProperty.Register(nameof(TotalCanvasDuration), typeof(TimeSpan), typeof(TimelineControl),
              new PropertyMetadata(null, TotalCanvasDurationChanged));

        private static void TotalCanvasDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeline = (TimelineControl)d;
            foreach (var item in timeline.timelineParentPanel.Children)
            {
                ((SingleTimeline)item).TotalCanvasDuration = (TimeSpan)e.NewValue;
            }
        }

        private void ScrollScaller_PositionChanged(object sender, PositionChangedEventArgs args)
        {
            ViewModel.TimelineWidth = rootGrid.ActualWidth * args.Scale;
            timelineScrollViewer.ChangeView(ViewModel.TimelineWidth * args.CurrentPosition, null, null);
            //timelineHeader.ChangeView(ViewModel.TimelineWidth * args.CurrentPosition, null, null);
        }

        private void timelineScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var offset = ((ScrollViewer)sender).HorizontalOffset;

            foreach (var item in timelineParentPanel.Children)
            {
                ((SingleTimeline)item).ScrollViewerHorizontalOffset = offset;
            }
        }

        // Manipulations
        private void UserControl_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
                var x = e.Delta.Translation.X;
                CurrentPosition += TotalCanvasDuration * (x / ViewModel.TimelineWidth);
        }
    }

    public class TimelineControlViewModel : ViewModels.ViewModelBase
    {
        private double _TimelineWidth;
        public double TimelineWidth
        {
            get { return _TimelineWidth; }
            set
            {
                _TimelineWidth = value;
                RaisePropertyChanged();
            }
        }

        private TimeSpan _CurrentPosition;
        public TimeSpan CurrentPosition
        {
            get { return _CurrentPosition; }
            set
            {
                _CurrentPosition = value;
                RaisePropertyChanged();
            }
        }
    }
}
