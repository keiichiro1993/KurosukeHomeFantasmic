using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthCommon.Models
{
    public interface IToken
    {
        UserType UserType { get; set; }

        string AccessToken { get; set; }
        string EntertainmentKey { get; set; }
        string RefreshToken { get; set; }
        DateTime TokenExpiration { get; set; }

        string Id { get; set; }

        bool IsTokenExpired();
    }
}
