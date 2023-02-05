using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
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

        public LEDPanelUnitSet LEDPanelUnitSet
        {
            get => (LEDPanelUnitSet)GetValue(LEDPanelUnitSetProperty);
            set => SetValue(LEDPanelUnitSetProperty, value);
        }

        public static readonly DependencyProperty LEDPanelUnitSetProperty =
          DependencyProperty.Register(nameof(LEDPanelUnitSet), typeof(LEDPanelUnitSet), typeof(PanelLayoutListItem),
              new PropertyMetadata(null, LEDPanelUnitSetPropertyChanged));

        private static void LEDPanelUnitSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public delegate void LEDPanelUnitSetDeleteButtonClickedEventHandler(object sender, ItemDeleteButtonClickedEventArgs<LEDPanelUnitSet> args);
        public event LEDPanelUnitSetDeleteButtonClickedEventHandler DeleteButtonClicked;

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteButtonClicked != null)
            {
                DeleteButtonClicked(this, new ItemDeleteButtonClickedEventArgs<LEDPanelUnitSet>(LEDPanelUnitSet));
            }
        }
    }
}
