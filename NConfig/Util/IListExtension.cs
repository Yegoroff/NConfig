using System.Configuration;
using System.Collections.Generic;

namespace NConfig
{
    internal static class IListExtension
    {

        public static IEnumerable<T> Reverse<T>(this IList<T> list) 
        {
            if (list == null)
                yield break;

            for (int i = list.Count - 1; i >= 0; i--)
                yield return list[i];            
        }
    }
}
