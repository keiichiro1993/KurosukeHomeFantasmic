using KurosukeHueClient.Models.HueObjects;
using Q42.HueApi.ColorConverters;
using System;
using System.Collections.Generic;
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
    public sealed partial class HueEffectListItem : UserControl
    {
        public HueEffectListItemViewModel ViewModel = new HueEffectListItemViewModel();
        public HueEffectListItem()
        {
            this.InitializeComponent();
        }
        public HueEffect Effect
        {
            get => (HueEffect)GetValue(EffectProperty);
            set => SetValue(EffectProperty, value);
        }

        public static readonly DependencyProperty EffectProperty =
          DependencyProperty.Register(nameof(Effect), typeof(HueEffect), typeof(HueEffectListItem),
              new PropertyMetadata(null, ActionPropertyChanged));

        private static void ActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (HueEffectListItem)d;
            if (e.NewValue != null)
            {
                instance.ViewModel.SetNewEffect((HueEffect)e.NewValue);
            }
        }


        public new bool CanDrag
        {
            get => (bool)GetValue(CanDragProperty);
            set => SetValue(CanDragProperty, value);
        }

        public static readonly new DependencyProperty CanDragProperty =
          DependencyProperty.Register(nameof(CanDrag), typeof(bool), typeof(HueEffectListItem), null);

        private void Grid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.Properties.Add("HueEffect", Effect);
            args.Data.Properties.Add("TimelineType", "Hue");
        }
    }

    public class HueEffectListItemViewModel : ViewModels.ViewModelBase
    {
        private HueEffect effect;
        public void SetNewEffect(HueEffect newEffect)
        {
            effect = newEffect;
            //RaisePropertyChanged("UIColor");
        }

        /*public SolidColorBrush UIColor
        {
            get
            {
                var color = action == null ? Colors.Black : Windows.UI.Color.FromArgb(255, (byte)(action.Color.R * 255), (byte)(action.Color.G * 255), (byte)(action.Color.B * 255));
                return new SolidColorBrush(color);
            }
        }*/
    }
}
