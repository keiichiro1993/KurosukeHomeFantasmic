using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeBonjourService.Utils
{
    internal static class IEnumerableExtension
    {
        internal static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }
    }
}
