using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace DCLSystem.Core.Caching.NetCache
{
  public  class WebCache
    {

        private static readonly Cache cache = HttpRuntime.Cache;
        public static readonly long DefaultExpireTime = 60L;
        public static readonly string KeySuffix = string.Empty;

        public static void Add(string key, object value)
        {
            Add(key, value, DefaultExpireTime);
        }

        public static void Add(string key, object value, bool flag)
        {
            if (flag)
            {
                Add(key, value, null, DateTime.Now.AddMinutes((double)DefaultExpireTime), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            else
            {
                Add(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((double)DefaultExpireTime), CacheItemPriority.Normal, null);
            }
        }

        public static void Add(string key, object value, long numOfMinutes)
        {
            Add(key, value, null, DateTime.Now.AddMinutes((double)numOfMinutes), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public static void Add(string key, object value, TimeSpan timeSpan)
        {
            Add(key, value, null, Cache.NoAbsoluteExpiration, timeSpan, CacheItemPriority.Normal, null);
        }

        public static void Add(string key, object value, CacheDependency dependencies)
        {
            if ((!string.IsNullOrEmpty(key) && (value != null)) && (!(value is DBNull) && (value != DBNull.Value)))
            {
                if ((dependencies is SqlCacheDependency) || (dependencies is AggregateCacheDependency))
                {
                    Add(key, value, dependencies, DateTime.Now.AddMinutes((double)DefaultExpireTime), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
                else
                {
                    Add(key, value, dependencies, DateTime.Now.AddMinutes((double)DefaultExpireTime), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
                }
            }
        }

        public static void Add(string key, object value, long expires, bool flag)
        {
            if (flag)
            {
                Add(key, value, null, DateTime.Now.AddMinutes((double)expires), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            else
            {
                Add(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((double)expires), CacheItemPriority.Normal, null);
            }
        }

        public static void Add(string key, object value, long expires, bool flag, CacheItemPriority priority)
        {
            if (flag)
            {
                Add(key, value, null, DateTime.Now.AddMinutes((double)expires), Cache.NoSlidingExpiration, priority, null);
            }
            else
            {
                Add(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((double)expires), priority, null);
            }
        }

        public static void Add<T>(IDictionary<string, T> items, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            if ((items != null) && (items.Count != 0))
            {
                foreach (KeyValuePair<string, T> pair in items)
                {
                    object obj2 = pair.Value;
                    if (obj2 != null)
                    {
                        Add(pair.Key, obj2, dependencies, absoluteExpiration, slidingExpiration, priority, callback);
                    }
                }
            }
        }

        public static void Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            if ((!string.IsNullOrEmpty(key) && (value != null)) && (!(value is DBNull) && (value != DBNull.Value)))
            {
                if (callback == null)
                {
                    callback = new CacheItemRemovedCallback(WebCache.RomovedCallBack);
                }
                cache.Add(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, callback);
            }
        }

        public static IDictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                return new Dictionary<string, T>();
            }
            var dictionary = new Dictionary<string, T>();
            IEnumerator<string> enumerator = keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                object obj2 = Get(current);
                if (obj2 is T)
                {
                    dictionary.Add(current, (T)obj2);
                }
            }
            return dictionary;
        }

        public static object Get(string key)
        {
            return cache[key];
        }

        public static T Get<T>(string key)
        {
            object obj2 = Get(key);
            if (obj2 is T)
            {
                return (T)obj2;
            }
            return default(T);
        }

        public static bool GetCacheTryParse(string key, out object obj)
        {
            if (string.IsNullOrEmpty(key))
            {
                obj = null;
                return false;
            }
            obj = Get(key);
            return (obj != null);
        }

        public static void Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                cache.Remove(key);
            }
        }

        public static void RemoveAll()
        {
            var enumerator = cache.GetEnumerator();
            var list = new ArrayList();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Key);
            }
            foreach (string str in list)
            {
                Remove(str);
            }
        }

        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            while (enumerator.MoveNext())
            {
                string input = enumerator.Key.ToString();
                if (regex.IsMatch(input))
                {
                    Remove(input);
                }
            }
        }

        public static void RomovedCallBack(string key, object value, CacheItemRemovedReason reason)
        {
            throw new NotImplementedException();
        }
    }
}
