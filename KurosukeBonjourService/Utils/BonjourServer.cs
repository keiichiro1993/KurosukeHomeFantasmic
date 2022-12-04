using CommonUtils;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeBonjourService.Utils
{
    public class BonjourServer
    {
        public static void StartServer(string instanceName = "Kurosuke Home Fantasmic!", string serviceName = "_fantasmic._tcp", ushort port = 11428)
        {
            var mdns = new MulticastService();
            mdns.QueryReceived += (s, e) =>
            {
                var names = e.Message.Questions
                    .Select(q => q.Name + " " + q.Type);
                DebugHelper.WriteDebugLog($"got a query for {String.Join(", ", names)}");
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
            mdns.Start(); // this thread will keep running in background (not disposing intentionally)
        }
    }
}
