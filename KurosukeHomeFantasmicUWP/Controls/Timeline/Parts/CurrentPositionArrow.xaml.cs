using CommonUtils;
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

namespace KurosukeHomeFantasmicUWP.Controls.Timeline.Parts
{
    public sealed partial class CurrentPositionArrow : UserControl
    {
        public CurrentPositionArrowViewModel ViewModel { get; set; } = new CurrentPositionArrowViewModel();
        public CurrentPositionArrow()
        {
            this.InitializeComponent();
        }

        public TimeSpan TotalCanvasDuration
        {
            get => (TimeSpan)GetValue(TotalCanvasDurationProperty);
            set => SetValue(TotalCanvasDurationProperty, value);
        }

        public static readonly DependencyProperty TotalCanvasDurationProperty =
          DependencyProperty.Register(nameof(TotalCanvasDuration), typeof(TimeSpan), typeof(CurrentPositionArrow),
              new PropertyMetadata(null, TotalCanvasDurationChanged));

        private static void TotalCanvasDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CurrentPositionArrow)d;
            control.ViewModel.TotalCanvasDuration = (TimeSpan)e.NewValue;
        }

        public TimeSpan CurrentPosition
        {
            get => (TimeSpan)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        public static readonly DependencyProperty CurrentPositionProperty =
          DependencyProperty.Register(nameof(CurrentPosition), typeof(TimeSpan), typeof(CurrentPositionArrow),
              new PropertyMetadata(null, CurrentPositionChanged));

        private static void CurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CurrentPositionArrow)d;
            control.ViewModel.CurrentPosition = (TimeSpan)e.NewValue;
        }

        public double CanvasWidth
        {
            get => (double)GetValue(CanvasWidthProperty);
            set => SetValue(CanvasWidthProperty, value);
        }

        public static readonly DependencyProperty CanvasWidthProperty =
          DependencyProperty.Register(nameof(CanvasWidth), typeof(double), typeof(CurrentPositionArrow),
              new PropertyMetadata(null, CanvasWidthChanged));

        private static void CanvasWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CurrentPositionArrow)d;
            control.ViewModel.CanvasWidth = (double)e.NewValue;
        }
    }

    public class CurrentPositionArrowViewModel : ViewModelBase
    {
        private double _CanvasWidth;
        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set
            {
                _CanvasWidth = value;
                RaisePropertyChanged("Left");
            }
        }

        private TimeSpan _TotalCanvasDuration;
        public TimeSpan TotalCanvasDuration
        {
            get { return _TotalCanvasDuration; }
            set
            {
                _TotalCanvasDuration = value;
                RaisePropertyChanged("Left");
            }
        }

        private TimeSpan _CurrentPosition;
        public TimeSpan CurrentPosition
        {
            get { return _CurrentPosition; }
            set
            {
                _CurrentPosition = value;
                RaisePropertyChanged("Left");
            }
        }

        public double Left { get { return CanvasWidth * (CurrentPosition / TotalCanvasDuration); } }
    }
}
