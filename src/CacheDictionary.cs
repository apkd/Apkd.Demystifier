using System.Collections.Generic;

namespace Apkd.Internal
{
    internal sealed class CacheDictionary<TKey, TValue> where TKey : class where TValue : class
    {
        readonly int cacheSize;
        readonly Queue<object> objectReferenceQueue;
        readonly System.Runtime.CompilerServices.ConditionalWeakTable<TKey, TValue> weakTable
            = new System.Runtime.CompilerServices.ConditionalWeakTable<TKey, TValue>();

        const int defaultCacheSize = 256;

        public CacheDictionary(int cacheSize = 256)
#if APKD_STACKTRACE_NOCACHE
            { }
#else
            => (this.cacheSize, objectReferenceQueue) = (cacheSize, new Queue<object>(capacity: cacheSize));
#endif

        public TValue GetOrInitializeValue(TKey key, System.Func<TKey, TValue> initializer)
        {
#if APKD_STACKTRACE_NOCACHE
            return initializer(key);
#else
            if (!weakTable.TryGetValue(key, out var value))
                weakTable.Add(key, value = initializer(key));
                
            if (objectReferenceQueue.Count > cacheSize)
                objectReferenceQueue.Dequeue();
            objectReferenceQueue.Enqueue(value);

            return value;
#endif
        }
    }
}
