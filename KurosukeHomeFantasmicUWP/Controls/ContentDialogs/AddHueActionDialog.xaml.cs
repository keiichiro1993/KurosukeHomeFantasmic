using KurosukeHomeFantasmicUWP.ViewModels;
using KurosukeHueClient.Models;
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

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.ContentDialogs
{
    public sealed partial class AddHueActionDialog : ContentDialog
    {
        public AddHueActionDialogViewModel ViewModel { get; set; } = new AddHueActionDialogViewModel();
        public AddHueActionDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddHueActionDialog_Loaded;
        }

        private void AddHueActionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.AddAction();
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }

    public class AddHueActionDialogViewModel : ViewModelBase
    {
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
        public HueAction Action { get; set; } = new HueAction
        {
            Color = new Q42.HueApi.ColorConverters.RGBColor(255,255,255),
            Brightness = 255,
            Duration = new TimeSpan(0, 0, 5)
        };

        public ObservableCollection<Light> SelectedLights = new ObservableCollection<Light>();

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
            LoadingMessage = "Retrieving Hue Entertainment Groups...";
            try
            {
                await GetHueLights();
            }
            catch (Exception ex)
            {
                await CommonUtils.DebugHelper.ShowErrorDialog(ex, "Failed to retrieve Hue Entertainment Groups.");
            }
            IsLoading = false;
        }

        public void AddAction()
        {
            Action.Name = Name;
            Action.Description = Description;
            Action.TargetLights = (from light in SelectedLights
                                  select light.HueEntertainmentLight).ToList();
            Utils.OnMemoryCache.HueActions.Add(Action);
        }

        private async System.Threading.Tasks.Task GetHueLights()
        {
            var hueUsers = from user in Utils.AppGlobalVariables.DeviceUsers
                           where user.UserType == AuthCommon.Models.UserType.Hue
                           select user as HueUser;

            if (hueUsers.Any())
            {
                if (!string.IsNullOrEmpty(Utils.AppGlobalVariables.CurrentProject.Settings.ActiveHueBridgeId))
                {
                    var matchedUsers = from user in hueUsers
                                       where user.Id == Utils.AppGlobalVariables.CurrentProject.Settings.ActiveHueBridgeId
                                       select user;

                    var hueUser = matchedUsers.FirstOrDefault();
                    if (hueUser != null)
                    {
                        Group hueGroup = null;
                        using (var client = new KurosukeHueClient.Utils.HueClient(hueUser))
                        {
                            var groups = await client.GetEntertainmentGroupsAsync();
                            if (!string.IsNullOrEmpty(Utils.AppGlobalVariables.CurrentProject.Settings.EntertainmentGroupId))
                            {
                                var matchedGroups = from hueGroupItem in groups
                                                    where hueGroupItem.HueGroup.Id == Utils.AppGlobalVariables.CurrentProject.Settings.EntertainmentGroupId
                                                    select hueGroupItem;
                                hueGroup = matchedGroups.FirstOrDefault();
                            }
                        }

                        if (hueGroup != null)
                        {
                            AvailableLights = new List<Light>(from device in hueGroup.Devices
                                                              select device as Light);
                        }
                        else
                        {
                            throw new Exception("Selected Hue Entertainment Group not found.");
                        }
                    }
                    else
                    {
                        throw new Exception("Selected bridge not found. Please re-check your configuration and network location.");
                    }
                }
            }
        }
    }
}
