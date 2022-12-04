using CommonUtils;
using KurosukeHueClient.Models.HueObjects;
using Q42.HueApi.ColorConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Hue
{
    public sealed partial class HueActionEditor : UserControl
    {
        public HueActionEditorViewModel ViewModel { get; set; } = new HueActionEditorViewModel();
        public HueActionEditor()
        {
            this.InitializeComponent();
        }

        public HueAction Action
        {
            get => (HueAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public static readonly DependencyProperty ActionProperty =
          DependencyProperty.Register(nameof(Action), typeof(HueAction), typeof(HueActionEditor),
              new PropertyMetadata(null, ActionPropertyChanged));

        private static void ActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (HueActionEditor)d;
            if (e.NewValue != null)
            {
                instance.ViewModel.SetAction((HueAction)e.NewValue);
            }
        }

        public bool MarginTextboxEnabled
        {
            get => (bool)GetValue(MarginTextboxEnabledProperty);
            set => SetValue(MarginTextboxEnabledProperty, value);
        }

        public static readonly DependencyProperty MarginTextboxEnabledProperty =
          DependencyProperty.Register(nameof(MarginTextboxEnabled), typeof(bool), typeof(HueActionEditor),
              new PropertyMetadata(false, MarginTextboxEnabledPropertyChanged));

        private static void MarginTextboxEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (HueActionEditor)d;
            instance.ViewModel.SetMarginTextboxEnabled((bool)e.NewValue);
        }
    }

    public class HueActionEditorViewModel : ViewModelBase
    {
        private HueAction action;
        public void SetAction(HueAction newAction)
        {
            action = newAction;
            RaisePropertyChanged("Color");
        }

        public void SetMarginTextboxEnabled(bool marginTextboxEnabled)
        {
            isMarginTextboxEnabled = marginTextboxEnabled;
            RaisePropertyChanged("MarginTextboxVisibility");
        }

        private bool isMarginTextboxEnabled;
        public Visibility MarginTextboxVisibility
        {
            get { return isMarginTextboxEnabled ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Color Color
        {
            get
            {
                if (action != null)
                {
                    return Color.FromArgb(255, (byte)action.Color.R, (byte)action.Color.G, (byte)action.Color.B);
                }
                else
                {
                    return Color.FromArgb(255, 255, 255, 255);
                }
            }
            set
            {
                action.Color = new RGBColor(BitConverter.ToString(new byte[] { value.R, value.G, value.B }).Replace("-", ""));
                action.Brightness = (double)Math.Max(value.R, Math.Max(value.G, value.B));
            }
        }
    }
}
