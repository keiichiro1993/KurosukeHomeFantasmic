using AuthCommon.Models;
using CommonUtils;
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
                try
                {
                    credentialList = vault.FindAllByResource(resourceNamePrefix + type.ToString());
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
                        cred.RetrievePassword();
                        taskList.Add(acquireAndAddUser(userList, type, cred));
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

        private static async Task acquireAndAddUser(List<IUser> userList, UserType type, PasswordCredential cred)
        {
            switch (type)
            {
                case UserType.Hue:
                    var token = new HueToken();
                    token.UserType = type;
                    token.Id = cred.UserName;
                    token.AccessToken = cred.Password;
                    var user = await HueAuthClient.FindHueBridge(token);
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
