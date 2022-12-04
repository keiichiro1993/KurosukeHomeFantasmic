using KurosukeBonjourService.Models;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KurosukeBonjourService.Utils
{
    public class BonjourClient
    {
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
    }
}
