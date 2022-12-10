using CommonUtils;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using Windows.Networking.Sockets;

namespace KurosukeBonjourService
{
    public class BonjourServer : IDisposable
    {
        private MulticastService mdns;
        public HttpServer Server { get; set; }
        public DatagramSocket UDPServer { get; set; }

        private string instanceName;
        private string serviceName;
        private ushort port;

        public BonjourServer(string instanceName = "Kurosuke Home Fantasmic!", string serviceName = "_fantasmic._tcp", ushort port = 11428)
        {
            this.instanceName = instanceName;
            this.serviceName = serviceName;
            this.port = port;
            Server = new HttpServer(port);
            UDPServer = new DatagramSocket();
        }

        public void StartTCPServer()
        {
            if (Server.WebSocketServices.Count == 0)
            {
                throw new InvalidOperationException("WebSocketServer has no service added. Please add service(s) before starting the server.");
            }

            // open websocket port
            Server.Start();

            // enable service discovery
            startBonjourServer(instanceName, serviceName, port);
        }

        public async Task StartUDPServer()
        {
            // open UDP port
            await UDPServer.BindServiceNameAsync(port.ToString());
        }

        private void startBonjourServer(string instanceName, string serviceName, ushort port)
        {
            if (mdns == null)
            {
                mdns = new MulticastService();
                mdns.QueryReceived += (s, e) =>
                {
                    var names = e.Message.Questions
                        .Where(x => x.Name.ToString().Contains(serviceName))
                        .Select(q => q.Name + " " + q.Type);
                    if (names.Any())
                    {
                        DebugHelper.WriteDebugLog($"got a query for {String.Join(", ", names)}");
                    }
                };
                mdns.NetworkInterfaceDiscovered += (s, e) =>
                {
                    foreach (var nic in e.NetworkInterfaces)
                    {
                        DebugHelper.WriteDebugLog($"discovered NIC '{nic.Name}'");
                    }
                };

                var serviceDiscovery = new ServiceDiscovery(mdns);
                var service = new ServiceProfile(instanceName, serviceName, port);
                service.AddProperty("role", "server");

                serviceDiscovery.Advertise(service);
            }

            mdns.Start(); // this thread will keep running in background (not disposing intentionally)
        }

        public void Dispose()
        {
            if (Server != null && Server.IsListening)
            {
                Server.Stop();
            }

            if (mdns != null)
            {
                mdns.Stop();
                mdns.Dispose();
            }
        }
    }
}
