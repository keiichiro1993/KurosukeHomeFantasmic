using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthCommon.Models
{
    public class TokenBase
    {
        public UserType UserType { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiration { get; set; }

        public string Id { get; set; }

        public bool IsTokenExpired()
        {
            return TokenExpiration == null || TokenExpiration - DateTime.Now < new TimeSpan(0, 0, 10);
        }
    }
}
