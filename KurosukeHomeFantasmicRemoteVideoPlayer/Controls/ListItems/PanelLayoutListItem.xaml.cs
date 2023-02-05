using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
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

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Controls.ListItems
{
    public sealed partial class PanelLayoutListItem : UserControl
    {
        public PanelLayoutListItem()
        {
            this.InitializeComponent();
        }

        internal LEDPanelUnitSet LEDPanelUnitSet
        {
            get => (LEDPanelUnitSet)GetValue(LEDPanelUnitSetProperty);
            set => SetValue(LEDPanelUnitSetProperty, value);
        }

        internal static readonly DependencyProperty LEDPanelUnitSetProperty =
          DependencyProperty.Register(nameof(LEDPanelUnitSet), typeof(LEDPanelUnitSet), typeof(PanelLayoutListItem),
              new PropertyMetadata(null, ActionPropertyChanged));

        private static void ActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
