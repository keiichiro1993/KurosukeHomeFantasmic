using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthCommon.Models
{
    public enum UserType { Hue }
    public interface IUser
    {
        string UserName { get; set; }
        TokenBase Token { get; set; }
        UserType UserType { get; set; }
        string ProfilePictureUrl { get; set; }
        string Id { get; set; }

        /*
        public async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you sure to delete account?", "Are you sure?");
            dialog.Commands.Add(new UICommand("Delete"));
            dialog.Commands.Add(new UICommand("Cancel"));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if (result.Label == "Delete")
            {
                await AccountManager.DeleteUser(this);
                AppGlobalVariables.Users.Remove(this);
            }
        }*/
    }
}
