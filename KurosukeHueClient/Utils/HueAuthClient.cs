using AuthCommon.Models;
using KurosukeHueClient.Models;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace KurosukeHueClient.Utils
{
    public class HueAuthClient
    {
        public static async Task<IEnumerable<LocatedBridge>> DiscoverHueBridges()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            return await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
        }

        public static async Task<HueUser> FindHueBridge(TokenBase token)
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

            var bridge = (from item in bridges
                          where item.BridgeId == token.Id.ToLower()
                          select item).FirstOrDefault();

            if (bridge != null)
            {
                var client = new LocalHueClient(bridge.IpAddress);
                client.Initialize(token.AccessToken);

                var bridgeInfo = await client.GetBridgeAsync();
                var bridgeId = bridgeInfo.Config.BridgeId;

                var user = new HueUser(bridgeInfo);
                user.Token = token;

                return user;
            }
            else
            {
                throw new InvalidOperationException("The Hue bridge with ID " + token.Id + " not found in current network.");
            }
        }

        public static async Task<HueUser> RegisterHueBridge()
        {
            var bridges = await DiscoverHueBridges();
            var clients = new List<LocalHueClient>();

            foreach (var bridge in bridges)
            {
                clients.Add(new LocalHueClient(bridge.IpAddress));
            }

            var hostname = (from name in NetworkInformation.GetHostNames()
                            where name.Type == Windows.Networking.HostNameType.DomainName
                            select name).First();

            var startTime = DateTime.Now;
            var timeout = new TimeSpan(0, 1, 0);

            while (DateTime.Now - startTime < timeout)
            {
                await Task.Delay(1500);

                foreach (var client in clients)
                {
                    try
                    {
                        var appKey = await client.RegisterAsync("KurosukeInfoBoard", hostname.DisplayName);
                        client.Initialize(appKey);
                        var bridgeInfo = await client.GetBridgeAsync();
                        var bridgeId = bridgeInfo.Config.BridgeId;

                        var token = new HueToken(appKey, bridgeId);
                        var user = new HueUser(bridgeInfo);
                        user.Token = token;

                        return user;
                    }
                    catch (LinkButtonNotPressedException ex)
                    {
                        //DebugHelper.Debugger.WriteDebugLog("Hue Bridge link button not yet pressed. " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //DebugHelper.Debugger.WriteErrorLog("Error while Hue Bridge registration. This might be because the list contains invalid bridge. Ignoring...", ex);
                    }
                }
            }

            throw new TimeoutException("Hue bridge discovery timed out. Please make sure you pressed the link button within 1 minite.");
        }
    }
}
