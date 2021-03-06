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
    public sealed partial class HueActionListItem : UserControl
    {
        public HueActionListItemViewModel ViewModel = new HueActionListItemViewModel();
        public HueActionListItem()
        {
            this.InitializeComponent();
        }
        public HueAction Action
        {
            get => (HueAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public static readonly DependencyProperty ActionProperty =
          DependencyProperty.Register(nameof(Action), typeof(HueAction), typeof(HueActionListItem),
              new PropertyMetadata(null, ActionPropertyChanged));

        private static void ActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (HueActionListItem)d;
            if (e.NewValue != null)
            {
                instance.ViewModel.SetNewAction((HueAction)e.NewValue);
            }
        }

        public new bool CanDrag
        {
            get => (bool)GetValue(CanDragProperty);
            set => SetValue(CanDragProperty, value);
        }

        public static readonly new DependencyProperty CanDragProperty =
          DependencyProperty.Register(nameof(CanDrag), typeof(bool), typeof(HueActionListItem), null);

        private void Grid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.Properties.Add("HueAction", Action);
            args.Data.Properties.Add("TimelineType", "Hue");
        }
    }

    public class HueActionListItemViewModel : ViewModels.ViewModelBase
    {
        private HueAction action;
        public void SetNewAction(HueAction newAction)
        {
            action = newAction;
            RaisePropertyChanged("UIColor");
        }

        public SolidColorBrush UIColor
        {
            get
            {
                var color = action == null ? Colors.Black : Windows.UI.Color.FromArgb(255, (byte)(action.Color.R * 255), (byte)(action.Color.G * 255), (byte)(action.Color.B * 255));
                return new SolidColorBrush(color);
            }
        }
    }
}
