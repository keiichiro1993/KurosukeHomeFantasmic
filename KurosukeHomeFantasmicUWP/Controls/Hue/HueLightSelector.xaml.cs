using CommonUtils;
using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace KurosukeHomeFantasmicUWP.Controls.Hue
{
    public sealed partial class HueLightSelector : UserControl
    {
        public HueLightSelector()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<Light> SelectedLights
        {
            get => (ObservableCollection<Light>)GetValue(SelectedLightsProperty);
            set => SetValue(SelectedLightsProperty, value);
        }

        public static readonly DependencyProperty SelectedLightsProperty =
          DependencyProperty.Register(nameof(SelectedLights), typeof(ObservableCollection<Light>), typeof(HueLightSelector),
              new PropertyMetadata(null, null));

        private ObservableCollection<LightItemViewModel> ListItems { get; set; } = new ObservableCollection<LightItemViewModel>();

        public IEnumerable<Light> AvailableLights
        {
            get => (IEnumerable<Light>)GetValue(AvailableLightsProperty);
            set => SetValue(AvailableLightsProperty, value);
        }

        public static readonly DependencyProperty AvailableLightsProperty =
          DependencyProperty.Register(nameof(AvailableLights), typeof(IEnumerable<Light>), typeof(HueLightSelector),
              new PropertyMetadata(null, AvailableLightsPropertyChanged));

        private static void AvailableLightsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (HueLightSelector)d;
            if (instance.AvailableLights != null)
            {
                foreach (var availableItem in instance.AvailableLights)
                {
                    var selected = false;
                    if (instance.SelectedLights != null)
                    {
                        var match = from item in instance.SelectedLights
                                    where item.HueLight.Id == availableItem.HueLight.Id
                                    select item;
                        selected = match.Any();
                    }
                    instance.ListItems.Add(new LightItemViewModel(availableItem, selected));
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SyncCheckboxStatus((CheckBox)sender);
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SyncCheckboxStatus((CheckBox)sender);
        }

        private void SyncCheckboxStatus(CheckBox sender)
        {
            var selectedItem = (LightItemViewModel)sender.DataContext;
            var match = from item in SelectedLights
                        where item.HueLight.Id == selectedItem.Light.HueLight.Id
                        select item;
            if (selectedItem.IsSelected)
            {
                if (!match.Any())
                {
                    SelectedLights.Add(selectedItem.Light);
                }
            }
            else
            {
                if (match.Any())
                {
                    SelectedLights.Remove(match.First());
                }
            }
        }

        private void lightListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (LightItemViewModel)e.ClickedItem;
            clickedItem.IsSelected = !clickedItem.IsSelected;
        }
    }

    public class LightItemViewModel : ViewModelBase
    {
        public LightItemViewModel(Light light, bool isSelected)
        {
            Light = light;
            IsSelected = isSelected;
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Light Light { get; set; }
    }
}
