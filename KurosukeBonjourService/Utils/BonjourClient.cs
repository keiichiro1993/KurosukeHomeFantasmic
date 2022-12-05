using CommonUtils;
using KurosukeBonjourService.Models;
using KurosukeBonjourService.Models.BonjourEventArgs;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace KurosukeBonjourService
{
    public class BonjourClient
    {
        public QueryResponseItem Device { get; }
        private WebSocket webSocket;
        private IPAddress validAddress;
        public BonjourClient(QueryResponseItem device)
        {
            Device = device;
        }

        public static async Task<List<QueryResponseItem>> QueryServiceAsync(string service = "_fantasmic._tcp.local")
        {
            var query = new Message();
            query.Questions.Add(new Question { Name = service, Type = DnsType.ANY });
            var cancellation = new CancellationTokenSource(2000);

            using (var mdns = new MulticastService())
            {
                mdns.Start();
                return QueryResponseItem.CreateFromQueryResult(await mdns.ResolveAsync(query, cancellation.Token));
            }
        }

        public async Task Connect()
        {
            // ignore if connected
            if (Status == ConnectionStatus.Connected ||
                webSocket?.ReadyState == WebSocketState.Connecting ||
                webSocket?.ReadyState == WebSocketState.Open)
            {
                return;
            }

            try
            {
                await findValidIP();
                webSocket = new WebSocket($"ws://{validAddress}:{Device.Port}");
                webSocket.Connect();
                // update status
                StatusMessage = "Success";
                Status = ConnectionStatus.Connected;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                throw ex;
            }
        }


        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;
        private ConnectionStatus _Status = ConnectionStatus.Disconnected;
        public ConnectionStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value;

                // fire event for notifying the view models
                if (ConnectionStatusChanged != null)
                {
                    ConnectionStatusChanged(this, new ConnectionStatusEventArgs
                    {
                        Status = Status,
                        StatusMessage = StatusMessage
                    });
                }
            }
        }
        public string StatusMessage { get; set; } = "Not connected";

        public async Task<string> Get(string path)
        {
            await findValidIP();
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            var uri = $"http://{validAddress}:{Device.Port}{path}";
            DebugHelper.WriteDebugLog($"GET Request to {uri}");
            var result = await httpClient.GetAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Request failed with status code {result.StatusCode}: {result.ReasonPhrase}");
            }
        }

        private async Task findValidIP()
        {
            if (validAddress != null)
            {
                return;
            }

            // find connectable ip address by bruteforce
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            foreach (var address in Device.IPv4Addresses)
            {
                var uri = $"http://{address}:{Device.Port}/alive";
                DebugHelper.WriteDebugLog($"GET Request to {uri}");
                try
                {
                    var result = await httpClient.GetAsync(uri);
                    if (result.IsSuccessStatusCode)
                    {
                        validAddress = address;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteErrorLog(ex, $"{uri} not available. Trying the next address.");
                }
            }

            throw new Exception($"Failed to find valid IP address for {Device.InstanceName}");
        }
    }
}
