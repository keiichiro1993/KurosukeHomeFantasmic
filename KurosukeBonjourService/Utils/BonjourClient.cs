using CommonUtils;
using KurosukeBonjourService.Models;
using KurosukeBonjourService.Models.BonjourEventArgs;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace KurosukeBonjourService
{
    public class BonjourClient
    {
        public QueryResponseItem Device { get; }
        private WebSocket webSocket;
        private DatagramSocket udpSocket;
        private HostName udpHostName;
        private IPAddress validAddress;
        public BonjourClient(QueryResponseItem device)
        {
            Device = device;
        }

        public static async Task<List<QueryResponseItem>> QueryServiceAsync(string service = "_fantasmic._tcp.local")
        {
            var query = new Message();
            query.Questions.Add(new Question { Name = service, Type = DnsType.ANY });
            var cancellation = new CancellationTokenSource(5000);

            using (var mdns = new MulticastService())
            {
                mdns.Start();
                return QueryResponseItem.CreateFromQueryResult(await mdns.ResolveAsync(query, cancellation.Token));
            }
        }

        #region WebSocket
        public async Task ConnectWebSocket()
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
                webSocket = new WebSocket($"ws://{validAddress}:{Device.Port}/play");
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

        public void DisconnectWebSocket()
        {
            webSocket?.Close();
            StatusMessage = "Disconnected";
            Status = ConnectionStatus.Disconnected;
        }

        public void SendWebSocketMessage<T>(T objectToSend)
        {
            if (Status != ConnectionStatus.Connected)
            {
                throw new InvalidOperationException("WebSocket not connected. Please make sure to connect before sending message.");
            }

            var jsonData = JsonSerializer.Serialize(objectToSend);
            webSocket.Send(jsonData);
        }
        #endregion

        #region UDPSocket (Datagram)
        public async Task ConnectUDPSocket()
        {
            // ignore if connected
            if (Status == ConnectionStatus.Connected ||
                udpSocket != null)
            {
                return;
            }

            try
            {
                await findValidIP();
                udpSocket = new DatagramSocket();
                udpHostName = new HostName(validAddress.ToString());
                //outputUdpStream = (await udpSocket.GetOutputStreamAsync(hostName, Device.Port.ToString())).AsStreamForWrite();

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

        public void DisconnectUDPSocket()
        {
            //outputUdpStream?.Dispose();
            udpSocket?.Dispose();
            udpSocket = null;
            StatusMessage = "Disconnected";
            Status = ConnectionStatus.Disconnected;
        }

        public async Task SendUDPMessage<T>(T objectToSend)
        {
            if (Status != ConnectionStatus.Connected)
            {
                throw new InvalidOperationException("UDPSocket not connected. Please make sure to connect before sending message.");
            }

            try
            {
                await SendUDPMessageInternal(objectToSend);
            }
            catch (ObjectDisposedException)
            {
                // Ignore as expected
                DebugHelper.WriteDebugLog("UDP object disposed.");
            }
        }

        private async Task SendUDPMessageInternal<T>(T objectToSend)
        {
            var jsonData = JsonSerializer.Serialize(objectToSend);
            using (var outputUdpStream = (await udpSocket.GetOutputStreamAsync(udpHostName, Device.Port.ToString())).AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputUdpStream))
                {
                    await streamWriter.WriteLineAsync(jsonData);
                    await streamWriter.FlushAsync();
                }
            }
        }
        #endregion

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
