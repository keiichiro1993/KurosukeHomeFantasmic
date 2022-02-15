using AuthCommon.Models;
using System;

namespace KurosukeHueClient.Models
{
    public class HueToken : IToken
    {
        public HueToken(string key, string entertainmentKey, string bridgeId)
        {
            AccessToken = key;
            EntertainmentKey = entertainmentKey;
            Id = bridgeId;
            UserType = UserType.Hue;
        }

        public HueToken() { }

        public UserType UserType { get; set; }

        public string AccessToken { get; set; }
        public string EntertainmentKey { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiration { get; set; }

        public string Id { get; set; }

        public bool IsTokenExpired()
        {
            return TokenExpiration == null || TokenExpiration - DateTime.Now < new TimeSpan(0, 0, 10);
        }
    }
}
