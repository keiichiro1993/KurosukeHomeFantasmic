using KurosukeHomeFantasmicUWP.Utils.RequestHelpers;
using KurosukeHomeFantasmicUWP.ViewModels;
using KurosukeHueClient.Models.HueObjects;
using Q42.HueApi.Streaming.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.ContentDialogs
{
    public sealed partial class AddHueEffectDialog : ContentDialog
    {
        public AddHueEffectDialogViewModel ViewModel { get; set; } = new AddHueEffectDialogViewModel();
        public AddHueEffectDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddHueEffectDialog_Loaded;
        }

        private void AddHueEffectDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.AddEffect();
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void AddActionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddAction();
        }
    }

    public class AddHueEffectDialogViewModel : ViewModelBase
    {
        public static HueAction HueAction(HueAction action) { return action; }

        private bool _IsPrimaryButtonEnabled = true;
        public bool IsPrimaryButtonEnabled
        {
            get { return _IsPrimaryButtonEnabled; }
            set
            {
                _IsPrimaryButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                checkButtonAvailability();
            }
        }

        public string Description { get; set; }

        public TimeSpan Margin { get; set; } = new TimeSpan(0, 0, 5);

        public IteratorEffectMode IteratorEffectMode { get; set; } = IteratorEffectMode.Bounce;

        public ObservableCollection<Light> SelectedLights = new ObservableCollection<Light>();
        public ObservableCollection<HueAction> HueActions = new ObservableCollection<HueAction>();
        
        private HueAction _NewHueAction = new HueAction
        {
            Color = new Q42.HueApi.ColorConverters.RGBColor(255, 255, 255),
            Brightness = 255,
            TransitionDuration = new TimeSpan(0, 0, 5),
            Margin = new TimeSpan(0, 0, 5)
        };
        public HueAction NewHueAction
        {
            get { return _NewHueAction; }
            set
            {
                _NewHueAction = value;
                RaisePropertyChanged();
            }
        }

        public void AddAction()
        {
            NewHueAction.Id = AddHueActionDialogViewModel.GetNewGuid();
            HueActions.Add(NewHueAction);

            NewHueAction = new HueAction
            {
                Color = new Q42.HueApi.ColorConverters.RGBColor(255, 255, 255),
                Brightness = 255,
                TransitionDuration = new TimeSpan(0, 0, 5),
                Margin = new TimeSpan(0, 0, 5)
            };
        }

        private List<Light> _AvailableLights;
        public List<Light> AvailableLights
        {
            get { return _AvailableLights; }
            set
            {
                _AvailableLights = value;
                RaisePropertyChanged();
            }
        }

        private void checkButtonAvailability()
        {
            var names = from action in Utils.OnMemoryCache.HueActions
                        select action.Name;
            IsPrimaryButtonEnabled = !string.IsNullOrEmpty(Name) && !names.Contains(Name);
        }


        public async void Init()
        {
            IsLoading = true;
            LoadingMessage = "Retrieving Hue Entertainment Lights...";
            try
            {
                AvailableLights = await HueRequestHelper.GetHueLights();
            }
            catch (Exception ex)
            {
                await CommonUtils.DebugHelper.ShowErrorDialog(ex, "Failed to retrieve Hue Entertainment Lights.");
            }
            IsLoading = false;
        }

        public HueEffect Effect { get; set; } = new HueEffect();
        public void AddEffect()
        {
            Effect.Id = getNewGuid();
            Effect.Name = Name;
            Effect.Description = Description;
            Effect.Margin = Margin;
            Effect.TargetLights = (from light in SelectedLights
                                   select light.HueEntertainmentLight).ToList();
            Effect.IteratorEffectMode = IteratorEffectMode;
            Effect.EffectMode = HueEffect.EffectModes.IteratorEffect;
            Utils.OnMemoryCache.HueEffects.Add(Effect);
        }

        private string getNewGuid()
        {
            var guid = Guid.NewGuid().ToString();
            var match = from item in Utils.OnMemoryCache.HueEffects
                        where item.Id == guid
                        select item;
            return match.Any() ? getNewGuid() : guid;
        }
    }
}
