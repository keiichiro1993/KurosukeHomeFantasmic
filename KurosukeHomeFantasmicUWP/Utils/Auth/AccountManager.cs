using AuthCommon.Models;
using CommonUtils;
using KurosukeHomeFantasmicUWP.Utils.DBHelpers;
using KurosukeHueClient.Models;
using KurosukeHueClient.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace KurosukeHomeFantasmicUWP.Utils.Auth
{
    public class AccountManager
    {
        private const string resourceNamePrefix = "KHomeFantasmic.";

        public static async Task<List<IUser>> GetAuthorizedUserList()
        {
            var userList = new List<IUser>();
            var taskList = new List<Task>();
            foreach (var type in Enum.GetValues(typeof(UserType)).Cast<UserType>())
            {
                IReadOnlyList<PasswordCredential> credentialList = null;
                var vault = new PasswordVault();
                var resourceName = resourceNamePrefix + type.ToString();
                try
                {
                    credentialList = vault.FindAllByResource(resourceName);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteErrorLog(ex, "There's no credential. Ignoring.");
                }

                if (credentialList != null && credentialList.Count() > 0)
                {
                    //Debugger.WriteDebugLog(credentialList.Count + "users found from credential vault.");
                    foreach (var cred in credentialList)
                    {
                        // Additional creds for each device type
                        PasswordCredential additionalCred = null;
                        switch (type)
                        {
                            case UserType.Hue:
                                resourceName += "EntertainmentKey";
                                additionalCred = vault.Retrieve(resourceName, cred.UserName);
                                break;
                        }

                        taskList.Add(acquireAndAddUser(userList, type, cred, additionalCred));
                    }
                }
            }

            await Task.WhenAll(taskList.ToArray());
            //Debugger.WriteDebugLog("Successfully retrieved tokens for " + userList.Count + " user(s).");
            return userList;
        }

        public static void DeleteUser(IUser user)
        {
            var resourceName = resourceNamePrefix + user.UserType.ToString();
            var vault = new PasswordVault();
            var cred = vault.Retrieve(resourceName, user.Id);
            vault.Remove(cred);

            // Additional creds for each device type
            switch (user.UserType)
            {
                case UserType.Hue:
                    resourceName += "EntertainmentKey";
                    cred = vault.Retrieve(resourceName, user.Id);
                    vault.Remove(cred);
                    break;
            }
        }

        private static async Task acquireAndAddUser(List<IUser> userList, UserType type, PasswordCredential cred, PasswordCredential additionalCred = null)
        {
            switch (type)
            {
                case UserType.Hue:
                    var token = new HueToken();
                    cred.RetrievePassword();
                    additionalCred.RetrievePassword();
                    token.UserType = type;
                    token.Id = cred.UserName;
                    token.AccessToken = cred.Password;
                    token.EntertainmentKey = additionalCred.Password;
                    //check if there's cached IP address for the bridge
                    var bridgeCacheHelper = new HueBridgeCacheHelper();
                    var previousIp = await bridgeCacheHelper.GetHueBridgeCachedIp(token.Id);
                    var user = await HueAuthClient.FindHueBridge(token, previousIp);
                    //save if ip address renewed
                    if (string.IsNullOrEmpty(previousIp) || previousIp != user.Bridge.Config.IpAddress)
                    {
                        await bridgeCacheHelper.SaveHueBridgeCache(token.Id, user.Bridge.Config.IpAddress);
                    }
                    userList.Add(user);
                    break;
            }
        }

        public static void SaveUserToVault(IUser user)
        {
            var resourceName = resourceNamePrefix + user.UserType.ToString();
            var vault = new PasswordVault();

            vault.Add(new PasswordCredential(resourceName, user.Id, user.Token.AccessToken));

            // Additional creds for each device type
            switch (user.UserType)
            {
                case UserType.Hue:
                    resourceName += "EntertainmentKey";
                    vault.Add(new PasswordCredential(resourceName, user.Id, user.Token.EntertainmentKey));
                    break;
            }
        }
    }
}
