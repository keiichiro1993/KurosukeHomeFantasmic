using CommonUtils;
using KurosukeBonjourService.Models;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KurosukeBonjourService
{
    public class BonjourClient
    {
        private QueryResponseItem device;
        public BonjourClient(QueryResponseItem device)
        {
            this.device = device;
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

        public async Task Connect(QueryResponseItem target)
        {

        }

        public async Task<string> Get(string path)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            foreach (var address in device.IPv4Addresses)
            {
                var uri = $"http://{address}:{device.Port}{path}";
                DebugHelper.WriteDebugLog($"GET Request to {uri}");
                try
                {
                    var result = await httpClient.GetAsync(uri);
                    if (result.IsSuccessStatusCode)
                    {
                        return await result.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteErrorLog(ex, $"{uri} not available. Trying the next address.");
                }
            }

            throw new Exception($"Failed to get '{path}' from {device.DomainName}:{device.Port}");
        }
    }
}
