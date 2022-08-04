using KurosukeHueClient.Models;
using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Utils.RequestHelpers
{
    public static class HueRequestHelper
    {
        public static async Task<List<Light>> GetHueLights()
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


                                if (hueGroup != null)
                                {
                                    return (from device in hueGroup.Devices
                                            select device as Light).ToList();
                                }
                                else
                                {
                                    throw new Exception("Selected Hue Entertainment Group not found.");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Entertainment Group not selected.");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Selected bridge not found. Please re-check your configuration and network location.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("No Hue Bridge selected. Please specify your bridge in the setting.");
                }
            }
            else
            {
                throw new InvalidOperationException("No Hue User found.");
            }
        }
    }
}
