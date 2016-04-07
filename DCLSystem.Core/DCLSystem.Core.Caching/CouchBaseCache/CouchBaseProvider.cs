using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration;
using DCLSystem.Core.Caching.HashAlgorithms;
using DCLSystem.Core.Caching.RedisCache;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using ProtoBuf;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ServiceStack.Text;

namespace DCLSystem.Core.Caching.CouchBaseCache
{
    [IdentifyCache(name: CacheTargetType.CouchBase)]
    public class CouchBaseProvider : ICacheProvider
    {
        #region 字段

        private static readonly ConcurrentDictionary<string, CouchbaseClient> Clients =
            new ConcurrentDictionary<string, CouchbaseClient>();

        private readonly Lazy<RedisContext> _context;
        private Lazy<long> _defaultExpireTime;
        private const double ExpireTime = 60D;
        private string _keySuffix;
        private Lazy<int> _connectTimeout;

        #endregion

        #region 构造函数

        public CouchBaseProvider(string appName)
        {
            _context = new Lazy<RedisContext>(() => CacheContainer.GetInstances<RedisContext>(appName));
            _keySuffix = appName;
            _defaultExpireTime = new Lazy<long>(() => long.Parse(_context.Value._defaultExpireTime));
            _connectTimeout = new Lazy<int>(() => int.Parse(_context.Value._connectTimeout));
        }

        public CouchBaseProvider()
        {
        }

        #endregion

        /// <summary>
        /// 添加K/V值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void Add(string key, object value)
        {
            this.Update(key, value);
        }

        /// <summary>
        /// 异步添加K/V值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void AddAsync(string key, object value)
        {
            Task.Run(() => this.Update(key, value));
        }

        /// <summary>
        /// 添加k/v值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="defaultExpire">默认配置失效时间</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void Add(string key, object value, bool defaultExpire)
        {
            this.Update(key, value,
                defaultExpire ? DefaultExpireTime : long.Parse(ExpireTime.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// 异步添加K/V值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="defaultExpire">默认配置失效时间</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void AddAsync(string key, object value, bool defaultExpire)
        {
            Task.Run(
                () =>
                    this.Update(key, value,
                        defaultExpire
                            ? DefaultExpireTime
                            : long.Parse(ExpireTime.ToString(CultureInfo.InvariantCulture))));
        }

        /// <summary>
        /// 添加k/v值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">默认配置失效时间</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void Add(string key, object value, long numOfMinutes)
        {
            this.Update(key, value, numOfMinutes);
        }

        /// <summary>
        /// 异步添加K/V值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">默认配置失效时间</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void AddAsync(string key, object value, long numOfMinutes)
        {
            Task.Run(() => this.Update(key, value, numOfMinutes));
        }

        /// <summary>
        /// 添加k/v值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">配置时间间隔</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void Add(string key, object value, TimeSpan timeSpan)
        {
            this.Update(key, value, timeSpan.Seconds);
        }

        /// <summary>
        /// 异步添加K/V值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">配置时间间隔</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void AddAsync(string key, object value, TimeSpan timeSpan)
        {
            Task.Run(() => this.Update(key, value, timeSpan.Seconds));
        }

        /// <summary>
        /// 添加k/v值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">配置时间间隔</param>
        /// <param name="flag">标识是否永不过期（NETCache本地缓存有效）</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void Add(string key, object value, long numOfMinutes, bool flag)
        {
            this.Update(key, value, numOfMinutes);
        }

        /// <summary>
        /// 异步添加K/V值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">配置时间间隔</param>
        /// <param name="flag">标识是否永不过期（NETCache本地缓存有效）</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void AddAsync(string key, object value, long numOfMinutes, bool flag)
        {
            Task.Run(() => this.Update(key, value, numOfMinutes));
        }

        /// <summary>
        /// 根据KEY键集合获取返回对象集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="keys">KEY值集合</param>
        /// <returns>需要返回的对象集合</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public IDictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                this.Add(key, this.Get<T>(key));
            }
            return result;
        }

        /// <summary>
        /// 根据KEY键集合异步获取返回对象集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="keys">KEY值集合</param>
        /// <returns>需要返回的对象集合</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public async Task<IDictionary<string, T>> GetAsync<T>(IEnumerable<string> keys)
        {
            var result = new Task<Dictionary<string, T>>(() => new Dictionary<string, T>());
            foreach (var key in keys)
            {
                this.Add(key, await Task.Run(() => this.Get<T>(key)));
            }
            return result.Result;
        }

        /// <summary>
        /// 根据KEY键获取返回对象
        /// </summary>
        /// <param name="key">KEY值</param>
        /// <returns>需要返回的对象</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public object Get(string key)
        {
            var node = GetRedisNode(key);
            var couchbase = GetCouchbaseClient(new CouchBaseEndpoint()
            {
                Db = node.Db,
                Host = node.Host,
                BucketPassword = node.Password,
                BucketName = node.UserName,
                Port = int.Parse(node.Port)
            });

            return couchbase.Get(GetKeySuffix(key));

        }

        /// <summary>
        /// 根据KEY异步获取返回对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<object> GetAsync(string key)
        {
            var result = await Task.Run(() => this.Get(key));
            return result;
        }

        /// <summary>
        /// 根据KEY键获取返回指定的类型对象
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="key">KEY值</param>
        /// <returns>需要返回的对象</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public T Get<T>(string key)
        {
            object obj2 = this.Get(key);
            if (obj2 == null)
            {
                return default(T);
            }
            if (obj2 is Stream)
            {
                return Serializer.Deserialize<T>(obj2 as Stream);
            }
            using (Stream stream = this.ToStream(obj2 as byte[]))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// 根据KEY异步获取指定的类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var result = await Task.Run(() => this.Get<T>(key));
            return result;
        }

        public bool GetCacheTryParse(string key, out object obj)
        {
            if (string.IsNullOrEmpty(key))
            {
                obj = null;
                return false;
            }
            obj = this.Get(key);
            return (obj != null);
        }

        /// <summary>
        /// 根据KEY键获取转化成指定的对象，指示获取转化是否成功的返回值
        /// </summary>
        /// <param name="key">KEY键</param>
        /// <param name="obj">需要转化返回的对象</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void Remove(string key)
        {
            IStoreOperationResult result = null;
            var node = GetRedisNode(key);
            var couchbase = GetCouchbaseClient(new CouchBaseEndpoint()
            {
                Db = node.Db,
                Host = node.Host,
                BucketPassword = node.Password,
                BucketName = node.UserName,
                Port = int.Parse(node.Port)
            });
            couchbase.Remove(GetKeySuffix(key));
        }

        /// <summary>
        /// 根据KEY键删除缓存
        /// </summary>
        /// <param name="key">KEY键</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/7</para>
        /// </remarks>
        public void RemoveAsync(string key)
        {
            Task.Run(() => this.Remove(key));
        }

        public bool Update(string key, object value, long numOfMinutes = -1L)
        {
            IStoreOperationResult result = null;
            var node = GetRedisNode(key);
            var couchBase = GetCouchbaseClient(new CouchBaseEndpoint()
            {
                Db = node.Db,
                Host = node.Host,
                BucketPassword = node.Password,
                BucketName = node.UserName,
                Port = int.Parse(node.Port)
            });
            var keySuffix = GetKeySuffix(key);
            if (numOfMinutes < 0L)
            {
                result = couchBase.ExecuteStore(StoreMode.Set, keySuffix, this.GetBytes(value));
            }
            else
            {
                var validFor = new TimeSpan(0, (int) numOfMinutes, 0);
                result = couchBase.ExecuteStore(StoreMode.Set, keySuffix, this.GetBytes(value), validFor);
            }
            if (result.Success)
            {
                return true;
            }
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            throw new Exception(string.Format("Couchbase Error Code: {0}", result.StatusCode));
        }

        public long DefaultExpireTime
        {
            get
            {
                return _defaultExpireTime.Value;
            }
            set
            {
                _defaultExpireTime = new Lazy<long>(() => value);
            }
        }


        public string KeySuffix
        {
            get
            {
                return _keySuffix;
            }
            set
            {
                _keySuffix = value;
            }
        }

        public int ConnectTimeout
        {
            get
            {
                return _connectTimeout.Value;
            }
            set
            {
                _connectTimeout = new Lazy<int>(() => value);
            }
        }

        #region 私有方法

        private CouchbaseClient GetCouchbaseClient(CouchBaseEndpoint info)
        {
            var key = info.SerializeToString();
            if (!Clients.ContainsKey(key))
            {
                var clientConfiguration = new CouchbaseClientConfiguration();

                var url = new Uri(string.Format("http://{0}:{1}/{2}", info.Host, info.Port, info.Db));
                clientConfiguration.Bucket = info.BucketName;
                clientConfiguration.BucketPassword = info.BucketPassword;
                clientConfiguration.Urls.Add(url);
                clientConfiguration.HttpRequestTimeout = TimeSpan.FromSeconds(ConnectTimeout);
                var couchbaseClient = new CouchbaseClient(clientConfiguration);
                Clients.GetOrAdd(key, couchbaseClient);
                return couchbaseClient;
            }
            else
            {
                return Clients[key];
            }
        }

        private Stream ToStream(byte[] bytes)
        {
            Stream stream4;
            using (var stream = new MemoryStream(bytes))
            {
                stream.Flush();
                stream.Position = 0L;
                var destination = new MemoryStream();
                using (var stream3 = new GZipStream(stream, CompressionMode.Decompress))
                {
                    stream3.Flush();
                    stream3.CopyTo(destination);
                    destination.Position = 0L;
                    stream4 = destination;
                }
            }
            return stream4;
        }

        private byte[] GetBytes(object o)
        {
            if (o == null)
            {
                return null;
            }
            using (var stream = new MemoryStream())
            {
                using (var stream2 = new MemoryStream())
                {
                    Serializer.Serialize<object>(stream2, o);
                    stream2.Position = 0L;
                    using (var stream3 = new GZipStream(stream, CompressionMode.Compress))
                    {
                        stream2.CopyTo(stream3);
                    }
                }
                return stream.ToArray();
            }
        }

        private string GetKeySuffix(string key)
        {
            return string.IsNullOrEmpty(KeySuffix) ? key : string.Format("_{0}_{1}", KeySuffix, key);
        }

        private ConsistentHashNode GetRedisNode(string item)
        {
            ConsistentHash<ConsistentHashNode> hash;
            _context.Value.dicHash.TryGetValue(CacheTargetType.CouchBase.ToString(), out hash);
            return hash != null ? hash.GetItemNode(item) : default(ConsistentHashNode);
        }

        #endregion
    }
}
