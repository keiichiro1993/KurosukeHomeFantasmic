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
using KurosukeBonjourService.Models.BonjourEventArgs;
using System.Text.Json.Serialization;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils
{
    internal static class BonjourHelper
    {
        public static event EventHandler<PlayVideoEventArgs> PlayVideoRequested;
        public static async Task StartServer()
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
            server.Server.AddWebSocketService("/play", () => {
                var videoService = new PlayVideoService();
                videoService.PlayVideoRequested += VideoService_PlayVideoRequested;
                return videoService;
            });

            // start server
            server.StartTCPServer();

            // start UDP server
            server.UDPServer.MessageReceived += UDPServer_MessageReceived;
            await server.StartUDPServer();
        }

        private static async void UDPServer_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
        {
            PlayVideoEventArgs playVideoData;
            using (var dataStream = args.GetDataStream()) 
            {
                playVideoData = await JsonSerializer.DeserializeAsync<PlayVideoEventArgs>(dataStream.AsStreamForRead());
            }

            if (playVideoData == null)
            {
                // skip if null
                return;
            }

            if (PlayVideoRequested != null)
            {
                PlayVideoRequested(sender, playVideoData);
            }
        }

        private static void VideoService_PlayVideoRequested(object sender, PlayVideoEventArgs e)
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
            switch (path) 
            {
                case "/videolist":
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
                    break;
                case "/alive":
                    res.ContentType = "text/plain";
                    var aliveContent = Encoding.GetEncoding("UTF-8").GetBytes("alive!");
                    res.ContentLength64 = aliveContent.Length;
                    res.ContentEncoding = Encoding.UTF8;
                    res.Close(aliveContent, true);
                    break;
                default:
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
            }

            
        }
    }
}
