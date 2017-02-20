using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using AutoMapper;
using System.Threading.Tasks;

namespace CSI.ModelHelper.Cache
{
    public static class CacheContext
    {
        private static Dictionary<string, CachedContent> Table;
        private static IMapper ObjectMapper;
        private static MapperConfiguration MapperConfig;

        public static int ExpireMinutes { get; set; }
        public static int MaxSize { get; set; }
        public static int PurgeSize { get; set; }

        static CacheContext()
        {
            ExpireMinutes = 30;
            MaxSize = 2000;
            PurgeSize = 500;
        }
        public static void Start(int expireMinutes = 30, int maxSize = 2000, int purgeSize = 500)
        {
            if (Table != null)
                throw new InvalidOperationException("Cache context is already started.");
            Table = new Dictionary<string, CachedContent>();

            if (null == MapperConfig)
            {
                MapperConfig = new MapperConfiguration(a => { a.CreateMissingTypeMaps = true; });
                ObjectMapper = MapperConfig.CreateMapper();
            }
            ExpireMinutes = expireMinutes;
            MaxSize = maxSize;
            PurgeSize = purgeSize;

            Task.Run(() => FlushThread());
        }
        public static bool ContainsKey(string key)
        {
            return Table.ContainsKey(key);
        }
        public static void SetValue(string key, object value, params string[] tags)
        {
            if (null != value)
            {
                Type t = value.GetType();
                value = ObjectMapper.Map(value, t, t);
            }
            lock (Table)
            {
                Table[key] = new CachedContent(value, tags);
            }
        }
        public static object GetValue(string key)
        {
            lock (Table)
            {
                if (Table.ContainsKey(key))
                {
                    object value = Table[key].Content;
                    if (null != value)
                    {
                        Type t = value.GetType();
                        return ObjectMapper.Map(value, t, t);
                    }
                }
            }
            return null;
        }
        public static object GetValueDirect(string key)
        {
            lock (Table)
            {
                if (Table.ContainsKey(key))
                {
                    return Table[key].Content;
                }
            }
            return null;
        }
        public static void FlushKey(string key)
        {
            lock (Table)
            {
                if (Table.ContainsKey(key))
                    Table.Remove(key);
            }
        }
        public static void FlushKeyStartsWith(string keyPhrase)
        {
            lock (Table)
            {
                var keys = Table.Keys.Where(k => k.StartsWith(keyPhrase)).ToList();
                keys.ForEach(k => Table.Remove(k));
            }
        }
        public static void FlushByTags(params string[] tags)
        {
            lock (Table)
            {
                var keys = Table.Where(k => k.Value.Tags.Any(a => tags.Contains(a)))
                    .Select(k => k.Key)
                    .ToList();
                keys.ForEach(k => Table.Remove(k));
            }
        }
        private static void FlushThread()
        {
            while (true)
            {
                Thread.Sleep(60 * 1000);
                DateTime deadline = DateTime.Now.AddMinutes(-ExpireMinutes);
                lock (Table)
                {
                    var expired = Table.Where(a => a.Value.TimeStamp < deadline).Select(a => a.Key).ToList();
                    expired.ForEach(a => Table.Remove(a));

                    if (Table.Count > MaxSize)
                    {
                        var purged = Table.OrderBy(a => a.Value.TimeStamp).Take(PurgeSize).Select(a => a.Key).ToList();
                        purged.ForEach(a => Table.Remove(a));
                    }
                }
            }
        }
    }
    internal class CachedContent
    {
        public DateTime TimeStamp { get; set; }
        public string[] Tags { get; set; }
        public object Content { get; set; }

        public CachedContent(object content, params string[] tags)
        {
            TimeStamp = DateTime.Now;
            Content = content;
            Tags = tags;
        }
    }
}
