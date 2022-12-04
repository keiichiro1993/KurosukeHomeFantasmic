using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KurosukeBonjourService;
using Windows.Networking.Connectivity;
using Windows.Networking;
using WebSocketSharp.Net;
using System.Text.Json;
using Windows.Storage.Streams;
using System.IO;
using System.Text.Encodings.Web;
using KurosukeBonjourService.Models.WebSocketServices;
using KurosukeBonjourService.Models.Json;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils
{
    internal static class BonjourHelper
    {
        public static event EventHandler<PlayVideoEventArgs> PlayVideoRequested;
        public static void StartServer()
        {
            // create server
            var hostName = NetworkInformation.GetHostNames().FirstOrDefault(name => name.Type == HostNameType.DomainName);
            if (string.IsNullOrEmpty(hostName?.DisplayName))
            {
                throw new Exception("Bonjour helper could not find the local host name for DNS broadcasting.");
            }
            var server = new BonjourServer(hostName.DisplayName);
            AppGlobalVariables.BonjourServer = server;

            // register services
            server.Server.OnGet += Server_OnGet;
            server.Server.AddWebSocketService<PlayVideoService>("/play", () => {
                var videoService = new PlayVideoService();
                videoService.PlayVideoRequested += VideoService_PlayVideoRequested;
                return videoService;
            });

            // start server
            server.StartServer();
        }

        private static void VideoService_PlayVideoRequested(object sender, KurosukeBonjourService.Models.Json.PlayVideoEventArgs e)
        {
            if (PlayVideoRequested != null)
            {
                PlayVideoRequested(sender, e);
            }
        }

        private async static void Server_OnGet(object sender, WebSocketSharp.Server.HttpRequestEventArgs e)
        {
            var req = e.Request;
            var res = e.Response;

            var path = req.RawUrl;

            // return without content if invalid path
            if (path != "/videolist")
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, AppGlobalVariables.VideoList, options);

            res.ContentType = "application/json";
            res.ContentEncoding = Encoding.UTF8;
            res.ContentLength64 = stream.Length;

            res.Close(stream.ToArray(), true);
        }
    }
}
