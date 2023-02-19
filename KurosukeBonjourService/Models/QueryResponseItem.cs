using KurosukeBonjourService.Utils;
using Makaretu.Dns;
using Makaretu.Dns.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;

namespace KurosukeBonjourService.Models
{
    public class QueryResponseItem
    {
        public static List<QueryResponseItem> CreateFromQueryResult(Message queryResult)
        {
            var items = new List<QueryResponseItem>();
            foreach (PTRRecord answer in queryResult.Answers)
            {
                var domainName = answer.DomainName.ToString();
                var instanceName = answer.DomainName.Labels[0];
                /*
                 * if the domain name is controller._fantasmic._tcp.local,
                 * canonical name will be controller.fantasmic.local for A/AAAA records
                 */
                var canonicalName = domainName.Replace("_tcp.", "").Replace("_", "");

                var port = (from record in queryResult.AdditionalRecords
                            where record.Type == DnsType.SRV && string.Equals(record.CanonicalName, domainName, StringComparison.OrdinalIgnoreCase)
                            select (record as SRVRecord).Port).First();

                var txtStrings = (from record in queryResult.AdditionalRecords
                                  where record.Type == DnsType.TXT && string.Equals(record.CanonicalName, domainName, StringComparison.OrdinalIgnoreCase)
                                  select (record as TXTRecord).Strings).FirstOrDefault();

                // expand attributes in txt
                var attributes = new Dictionary<string, string>();
                foreach (var txt in txtStrings.OrEmptyIfNull())
                {
                    var keyValue = txt.Split('=');
                    attributes.Add(keyValue[0], keyValue[1]);
                }

                var ipv4 = from record in queryResult.AdditionalRecords
                           where record.Type == DnsType.A && string.Equals(record.CanonicalName, canonicalName, StringComparison.OrdinalIgnoreCase)
                           select (record as ARecord).Address;
                var ipv6 = from record in queryResult.AdditionalRecords
                           where record.Type == DnsType.AAAA && string.Equals(record.CanonicalName, canonicalName, StringComparison.OrdinalIgnoreCase)
                           select (record as AAAARecord).Address;

                items.Add(new QueryResponseItem
                {
                    InstanceName = instanceName,
                    DomainName = domainName,
                    CanonicalName = canonicalName,
                    Port = port,
                    Attributes = attributes,
                    IPv4Addresses = ipv4.ToList(),
                    IPv6Addresses = ipv6.ToList()
                });
            }
            return items;
        }

        public string InstanceName { get; set; }
        public string DomainName { get; set; }
        public string CanonicalName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public ushort Port { get; set; }
        public List<IPAddress> IPv4Addresses { get; set; }
        public List<IPAddress> IPv6Addresses { get; set; }
    }
}
