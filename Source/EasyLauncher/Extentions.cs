using System.Collections.Concurrent;

namespace EasyLauncher
{
    public static class Extentions
    {
        public static void Remove<TKey>(this ConcurrentDictionary<TKey, object> dictionary, TKey key)
        {
            object value;
            dictionary.TryRemove(key, out value);
        }

        public static void Add<TKey>(this ConcurrentDictionary<TKey, object> dictionary, TKey key)
        {
            dictionary.TryAdd(key, null);
        }
    }
}