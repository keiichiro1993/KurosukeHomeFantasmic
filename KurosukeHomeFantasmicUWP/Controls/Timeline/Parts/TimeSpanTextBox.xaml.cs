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
    public sealed partial class TimeSpanTextBox : UserControl
    {
        public TimeSpanTextBox()
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
              new PropertyMetadata(null, DurationChanged));

        private static void DurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
