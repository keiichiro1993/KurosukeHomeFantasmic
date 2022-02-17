using KurosukeHueClient.Models;
using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.Settings
{
    public class HueSettingsPageViewModel : ViewModelBase
    {
        private bool _IsUIAvailable = false;
        public bool IsUIAvailable
        {
            get { return _IsUIAvailable; }
            set
            {
                _IsUIAvailable = value;
                RaisePropertyChanged();
            }
        }

        private List<HueUser> _AvailableHueBridges;
        public List<HueUser> AvailableHueBridges
        {
            get { return _AvailableHueBridges; }
            set
            {
                _AvailableHueBridges = value;
                RaisePropertyChanged();
            }
        }
        private HueUser _SelectedHueBridge;
        public HueUser SelectedHueBridge
        {
            get { return _SelectedHueBridge; }
            set
            {
                _SelectedHueBridge = value;
                Utils.AppGlobalVariables.CurrentProject.Settings.ActiveHueBridgeId = value.Id;
                SelectedBridgeChanged();
            }
        }

        private List<Group> _HueGroups;
        public List<Group> HueGroups
        {
            get { return _HueGroups; }
            set
            {
                if (value != null)
                {
                    _HueGroups = value;
                    RaisePropertyChanged();
                }
            }
        }
        private Group _SelectedHueGroup;
        public Group SelectedHueGroup
        {
            get { return _SelectedHueGroup; }
            set
            {
                if (value != null)
                {
                    _SelectedHueGroup = value;
                    Utils.AppGlobalVariables.CurrentProject.Settings.EntertainmentGroupId = value.HueGroup.Id;
                }
            }
        }

        public async Task Init()
        {
            IsLoading = true;
            var hueUsers = from user in Utils.AppGlobalVariables.DeviceUsers
                           where user.UserType == AuthCommon.Models.UserType.Hue
                           select user as HueUser;

            if (hueUsers.Any())
            {
                AvailableHueBridges = hueUsers.ToList();
                if (!string.IsNullOrEmpty(Utils.AppGlobalVariables.CurrentProject.Settings.ActiveHueBridgeId))
                {
                    var matchedBridges = from hueGroup in hueUsers
                                         where hueGroup.Id == Utils.AppGlobalVariables.CurrentProject.Settings.ActiveHueBridgeId
                                         select hueGroup;

                    var matchedBridge = matchedBridges.FirstOrDefault();
                    if (matchedBridge != null)
                    {
                        _SelectedHueBridge = matchedBridge;
                        RaisePropertyChanged("SelectedHueBridge");
                        await RetrieveGroups(matchedBridge);

                        if (!string.IsNullOrEmpty(Utils.AppGlobalVariables.CurrentProject.Settings.EntertainmentGroupId))
                        {
                            var matchedGroups = from hueGroup in HueGroups
                                                where hueGroup.HueGroup.Id == Utils.AppGlobalVariables.CurrentProject.Settings.EntertainmentGroupId
                                                select hueGroup;
                            var matchedGroup = matchedGroups.FirstOrDefault();
                            if (matchedGroup != null)
                            {
                                SelectedHueGroup = matchedGroup;
                                RaisePropertyChanged("SelectedHueGroup");
                            }
                            else
                            {
                                //TODO: error
                            }
                        }
                    }
                    else
                    {
                        //TODO: error
                    }
                }
                IsUIAvailable = true;
            }
            IsLoading = false;
        }

        public async void SelectedBridgeChanged()
        {
            IsLoading = true;
            IsUIAvailable = false;
            SelectedHueGroup = null;
            RaisePropertyChanged("SelectedHueGroup");
            await RetrieveGroups(SelectedHueBridge);
            IsUIAvailable = true;
            IsLoading = false;
        }

        private async Task RetrieveGroups(HueUser bridge)
        {
            try
            {
                using (var client = new KurosukeHueClient.Utils.HueClient(bridge))
                {
                    HueGroups = await client.GetEntertainmentGroupsAsync();
                }
            }
            catch (Exception ex)
            {
                await Utils.DebugHelper.ShowErrorDialog(ex, "Failed to get Entertainment Groups for " + bridge.UserName);
            }
        }
    }
}
