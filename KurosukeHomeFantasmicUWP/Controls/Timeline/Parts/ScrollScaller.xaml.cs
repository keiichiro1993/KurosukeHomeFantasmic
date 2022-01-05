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

namespace KurosukeHomeFantasmicUWP.Controls.Timeline
{
    public sealed partial class ScrollScaller : UserControl
    {
        ScrollScallerViewModel ViewModel = new ScrollScallerViewModel();
        public ScrollScaller()
        {
            this.InitializeComponent();
            this.Loaded += ScrollScaller_Loaded;
        }

        private void ScrollScaller_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CanvasWidth = scalerCanvas.ActualWidth;
            ViewModel.Width = ViewModel.CanvasWidth;
        }

        public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs args);
        public event PositionChangedEventHandler PositionChanged;

        private void triggerPositionChangedEvent()
        {
            if (this.PositionChanged != null && ViewModel.Width != 0)
            {
                this.PositionChanged(this, new PositionChangedEventArgs(ViewModel.CurrentPosition, ViewModel.Scale));
            }
        }

        // Manipulations
        private void UserControl_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            var x = e.Delta.Translation.X;
            ViewModel.Left += x;
            triggerPositionChangedEvent();
        }

        private void UserControl_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            this.Opacity = 0.5;
        }

        private void UserControl_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            this.Opacity = 1;
        }



        private void ResizeButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 10);
        }

        private void ResizeButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void ResizeLeftButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            var x = e.Delta.Translation.X;
            ViewModel.ChangeLeftOnly(x);
            triggerPositionChangedEvent();
        }

        private void ResizeRightButton_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            var x = e.Delta.Translation.X;
            ViewModel.Width += x;
            triggerPositionChangedEvent();
        }

        private void scalerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.CanvasWidth = scalerCanvas.ActualWidth;
            triggerPositionChangedEvent();
        }
    }

    public class PositionChangedEventArgs : System.EventArgs
    {
        private double currentPosition;
        private double scale;

        public PositionChangedEventArgs(double currentPosition, double scale)
        {
            this.currentPosition = currentPosition;
            this.scale = scale;
        }

        public double CurrentPosition
        {
            get { return currentPosition; }
        }
        public double Scale
        {
            get { return scale; }
        }
    }

    public class ScrollScallerViewModel : ViewModels.ViewModelBase
    {
        // Position on Canvas
        private double _Width;
        public double Width
        {
            get { return _Width; }
            set
            {
                _Width = value >= 50 ? (_Left + value <= CanvasWidth ? value : CanvasWidth - _Left) : 50;
                RaisePositionPropertyChanged();
            }
        }
        private double _Left;
        public double Left
        {
            get { return _Left; }
            set
            {
                _Left = value + _Width <= CanvasWidth ? (0 <= value ? value : 0) : CanvasWidth - _Width;
                RaisePositionPropertyChanged();
            }
        }

        public void ChangeLeftOnly(double x)
        {
            if (Left + x >= 0 && Width - x >= 50)
            {
                _Left += x;
                _Width -= x;
                RaisePositionPropertyChanged();
            }
        }

        private void RaisePositionPropertyChanged()
        {
            RaisePropertyChanged("Left");
            RaisePropertyChanged("Width");
            RaisePropertyChanged("CanvasWidth");
            RaisePropertyChanged("CurrentPosition");
        }

        private double _CanvasWidth;
        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set
            {
                _CanvasWidth = value;
            }
        }

        public double Scale
        {
            get { return CanvasWidth / Width; }
        }

        public double CurrentPosition
        {
            get { return Left / CanvasWidth; }
        }
    }
}
