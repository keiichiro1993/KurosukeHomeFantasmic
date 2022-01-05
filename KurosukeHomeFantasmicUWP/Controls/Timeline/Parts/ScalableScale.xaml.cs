using KurosukeHomeFantasmicUWP.Controls.Timeline.Parts;
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
    public sealed partial class ScalableScale : UserControl
    {
        public ScalableScale()
        {
            this.InitializeComponent();
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
            var scale = (ScalableScale)d;
            var children = scale.scaleCanvas.Children;
            foreach (var child in children)
            {
                ((ScaleLine)child).ViewModel.TotalCanvasDuration = (TimeSpan)e.NewValue;
            }
        }

        private void scaleCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = scaleCanvas.ActualWidth;
            var children = scaleCanvas.Children;
            var distance = width / children.Count();
            if (distance < 8 || distance > 12)
            {
                scaleCanvas.Children.Clear();
                var count = width / 10;
                var span = TotalCanvasDuration / count;
                for (int i = 0; i <= count; i++)
                {
                    var line = new ScaleLine(TotalCanvasDuration, span * i, width);
                    scaleCanvas.Children.Add(line);
                }
            }
            else
            {
                foreach (var child in children)
                {
                    ((ScaleLine)child).ViewModel.ScaleWidth = width;
                }
            }
        }
    }
}
