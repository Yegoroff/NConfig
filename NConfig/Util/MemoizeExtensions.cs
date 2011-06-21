using System.Collections.Generic;
using System;

namespace NConfig
{
    internal static class MemoizeExtensions
    {
        public static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> f)
        {
            // We can apply recursive Memoization but it will be quite cumbersome

            var mem = new Dictionary<TArg1, Dictionary<TArg2, TResult>>();

            return (a1, a2) =>
            {
                Dictionary<TArg2, TResult> dict;
                if (!mem.TryGetValue(a1, out dict)) {
                    dict = new Dictionary<TArg2,TResult>();
                    mem.Add(a1, dict);
                }
                TResult value;
                if (!dict.TryGetValue(a2, out value))
                {
                    value = f(a1, a2);
                    dict.Add(a2, value);
                }
                return value;
            };
        }

        public static Func<TArg, TResult> Memoize<TArg, TResult>(this Func<TArg, TResult> f)
        {
            var mem = new Dictionary<TArg, TResult>();

            return a =>
            {
                TResult value;
                if (!mem.TryGetValue(a, out value))
                {
                    value = f(a);
                    mem.Add(a, value);
                }
                return value;
            };
        }
    }
}
