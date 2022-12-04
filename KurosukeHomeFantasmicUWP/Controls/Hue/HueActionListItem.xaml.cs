using CommonUtils;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
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

        public delegate void ActionDeleteButtonClickedEventHandler(object sender, ItemDeleteButtonClickedEventArgs<HueAction> args);
        public event ActionDeleteButtonClickedEventHandler DeleteButtonClicked;

        public bool MarginTextboxEnabled
        {
            get => (bool)GetValue(MarginTextboxEnabledProperty);
            set => SetValue(MarginTextboxEnabledProperty, value);
        }

        public static readonly DependencyProperty MarginTextboxEnabledProperty =
          DependencyProperty.Register(nameof(MarginTextboxEnabled), typeof(bool), typeof(HueActionListItem),
              new PropertyMetadata(false, MarginTextboxEnabledPropertyChanged));

        private static void MarginTextboxEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (HueActionListItem)d;
            instance.ViewModel.SetMarginTextboxEnabled((bool)e.NewValue);
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

        private void ContextMenuDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteButtonClicked != null)
            {
                this.DeleteButtonClicked(this, new ItemDeleteButtonClickedEventArgs<HueAction>(Action));
            }
        }
    }

    public class HueActionListItemViewModel : ViewModelBase
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

        public void SetMarginTextboxEnabled(bool marginTextboxEnabled)
        {
            isMarginTextboxEnabled = marginTextboxEnabled;
            RaisePropertyChanged("MarginVisibility");
            RaisePropertyChanged("NameVisibility");
        }

        private bool isMarginTextboxEnabled;
        public Visibility MarginVisibility
        {
            get { return isMarginTextboxEnabled ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility NameVisibility
        {
            get { return isMarginTextboxEnabled ? Visibility.Collapsed : Visibility.Visible; }
        }
    }
}
