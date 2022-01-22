using AuthCommon.Models;

namespace KurosukeHueClient.Models
{
    public class HueToken : TokenBase
    {
        public HueToken(string key, string bridgeId)
        {
            AccessToken = key;
            UserType = UserType.Hue;
            Id = bridgeId;
        }

        public HueToken() { }
    }
}
