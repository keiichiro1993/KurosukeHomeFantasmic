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
        IToken Token { get; set; }
        UserType UserType { get; set; }
        string ProfilePictureUrl { get; set; }
        string Id { get; set; }
    }
}
