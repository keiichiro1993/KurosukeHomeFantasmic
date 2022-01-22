using AuthCommon.Models;
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
        private const string ResourceNamePrefix = "KHomeFantasmic.";

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
                    credentialList = vault.FindAllByResource(ResourceNamePrefix + type.ToString());
                }
                catch (Exception ex)
                {
                    //Debugger.WriteErrorLog("There's no credential. Ignoring.", ex);
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
            var vault = new PasswordVault();
            var cred = vault.Retrieve(ResourceNamePrefix + user.UserType.ToString(), user.Id);
            vault.Remove(cred);
        }

        private static async Task acquireAndAddUser(List<IUser> userList, UserType type, PasswordCredential cred)
        {
            var token = new TokenBase();
            token.UserType = type;
            token.Id = cred.UserName;
            token.AccessToken = cred.Password;
            switch (type)
            {
                case UserType.Hue:
                    var user = await HueAuthClient.FindHueBridge(token);
                    userList.Add(user);
                    break;
            }
        }

        public static void SaveUserToVault(IUser user)
        {
            var resourceName = ResourceNamePrefix + user.UserType.ToString();
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(resourceName, user.Id, user.Token.AccessToken));
        }
    }
}
