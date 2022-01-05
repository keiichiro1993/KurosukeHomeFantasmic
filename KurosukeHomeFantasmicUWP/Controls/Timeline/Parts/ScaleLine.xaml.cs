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
    public sealed partial class ScaleLine : UserControl
    {
        public ScaleLineViewModel ViewModel { get; set; } = new ScaleLineViewModel();
        public ScaleLine(TimeSpan totalDuration, TimeSpan position, double scaleWidth)
        {
            this.InitializeComponent();
            ViewModel.TotalCanvasDuration = totalDuration;
            ViewModel.ScaleWidth = scaleWidth;
            ViewModel.CurrentPosition = position;
            this.SizeChanged += ScaleLine_SizeChanged;
        }

        private void ScaleLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.MyWidth = e.NewSize.Width;
        }
    }

    public class ScaleLineViewModel : ViewModels.ViewModelBase
    {
        public string CurrentPositionText
        {
            get
            {
                return (CurrentPosition.Seconds == 0 && CurrentPosition.Milliseconds < 300) ? CurrentPosition.ToString(@"hh\:mm\:ss") :
                    (CurrentPosition.Seconds == 59 && CurrentPosition.Milliseconds > 700) ? CurrentPosition.Add(TimeSpan.FromSeconds(1)).ToString(@"hh\:mm\:ss") : null;
            }
        }

        private TimeSpan _CurrentPosition;
        public TimeSpan CurrentPosition
        {
            get
            {
                return _CurrentPosition;
            }
            set
            {
                _CurrentPosition = value;
                RaisePropertyChanged("CurrentPositionText");
                RaisePropertyChanged("Row");
                RaisePropertyChanged("RowSpan");
            }
        }

        private TimeSpan _TotalCanvasDuration;
        public TimeSpan TotalCanvasDuration
        {
            get
            {
                return _TotalCanvasDuration;
            }
            set
            {
                _TotalCanvasDuration = value;
                RaisePropertyChanged("Left");
            }
        }

        private double _ScaleWidth;
        public double ScaleWidth
        {
            get { return _ScaleWidth; }
            set
            {
                _ScaleWidth = value;
                RaisePropertyChanged("Width");
                RaisePropertyChanged("Left");
            }
        }

        private double _MyWidth;
        public double MyWidth
        {
            get { return _MyWidth; }
            set
            {
                _MyWidth = value;
                RaisePropertyChanged("Left");
            }
        }
        public double Left
        {
            get
            {
                return ScaleWidth * (CurrentPosition / TotalCanvasDuration) - (MyWidth / 2);
            }
        }

        public int Row
        {
            get { return string.IsNullOrEmpty(CurrentPositionText) ? 2 : 1; }
        }

        public int RowSpan
        {
            get { return string.IsNullOrEmpty(CurrentPositionText) ? 1 : 2; }
        }
    }
}
