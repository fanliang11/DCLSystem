using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using DCLSystem.Core.Caching.RedisCache;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace DCLSystem.Core.Caching.NetCache
{
    [IdentifyCache(name: CacheTargetType.WebCache)]
    public sealed class WebCacheProvider : ICacheProvider
    {
        #region 字段

        /// <summary>
        /// 缓存数据上下文
        /// </summary>
        private readonly Lazy<RedisContext> _context;

        /// <summary>
        /// 默认失效时间
        /// </summary>
        private Lazy<long> _defaultExpireTime;

        /// <summary>
        /// 配置失效时间
        /// </summary>
        private const double ExpireTime = 60D;

        /// <summary>
        /// KEY键前缀
        /// </summary>
        private string _keySuffix;

        #endregion

        #region 构造函数

        public WebCacheProvider(string appName)
        {
            _context = new Lazy<RedisContext>(() => CacheContainer.GetInstances<RedisContext>(appName));
            _keySuffix = appName;
            _defaultExpireTime = new Lazy<long>(() => long.Parse(_context.Value._defaultExpireTime));
        }

        public WebCacheProvider()
        {
            _defaultExpireTime = new Lazy<long>(() => 60);
            _keySuffix = string.Empty;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public void Add(string key, object value)
        {

            WebCache.Add(GetKeySuffix(key), value, this.DefaultExpireTime);
        }

        /// <summary>
        /// 异步添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public void AddAsync(string key, object value)
        {
            Task.Run(() => WebCache.Add(GetKeySuffix(key), value, DefaultExpireTime));
        }

        /// <summary>
        /// 添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="flag">缓存是否永不过期</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public void Add(string key, object value, bool flag)
        {
            WebCache.Add(GetKeySuffix(key), value, flag);
        }

        /// <summary>
        /// 异步添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="flag">缓存是否永不过期</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void AddAsync(string key, object value, bool flag)
        {
            Task.Run(() => WebCache.Add(GetKeySuffix(key), value, flag));
        }

        /// <summary>
        /// 添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">失效时间</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Add(string key, object value, long numOfMinutes)
        {
            WebCache.Add(GetKeySuffix(key), value, null, DateTime.Now.AddMinutes((double)numOfMinutes), Cache.NoSlidingExpiration,
                CacheItemPriority.High, null);
        }

        /// <summary>
        /// 异步添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">失效时间</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void AddAsync(string key, object value, long numOfMinutes)
        {
            Task.Run(
                () =>
                    WebCache.Add(GetKeySuffix(key), value, null, DateTime.Now.AddMinutes((double)numOfMinutes),
                        Cache.NoSlidingExpiration, CacheItemPriority.High, null));
        }

        /// <summary>
        /// 添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">配置时间间隔</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Add(string key, object value, TimeSpan timeSpan)
        {
            WebCache.Add(GetKeySuffix(key), value, null, Cache.NoAbsoluteExpiration, timeSpan, CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// 异步添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">配置时间间隔</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void AddAsync(string key, object value, TimeSpan timeSpan)
        {
            Task.Run(
                () =>
                    WebCache.Add(GetKeySuffix(key), value, null, Cache.NoAbsoluteExpiration, timeSpan, CacheItemPriority.Normal, null));
        }

        /// <summary>
        /// 添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">失效时间</param>
        /// <param name="flag">缓存是否永不过期</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Add(string key, object value, long numOfMinutes, bool flag)
        {
            WebCache.Add(GetKeySuffix(key), value, numOfMinutes, flag);
        }

        /// <summary>
        /// 异步添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="numOfMinutes">失效时间</param>
        /// <param name="flag">缓存是否永不过期</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void AddAsync(string key, object value, long numOfMinutes, bool flag)
        {
            Task.Run(() => WebCache.Add(GetKeySuffix(key), value, numOfMinutes, flag));
        }

        /// <summary>
        /// 添加K/V本地缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">失效时间</param>
        /// <param name="flag">缓存是否永不过期</param>
        /// <param name="priority">指定 System.Web.Caching.Cache 对象中存储的项的相对优先级。</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Add(string key, object value, long expires, bool flag, CacheItemPriority priority)
        {
            WebCache.Add(GetKeySuffix(key), value, expires, flag, priority);
        }

        /// <summary>
        /// 添加K/V集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="items"> 键/值对的泛型集合。</param>
        /// <param name="dependencies">该项的文件依赖项或缓存键依赖项。 当任何依赖项更改时，该对象即无效，并从缓存中移除。 如果没有依赖项，则此参数包含 null。</param>
        /// <param name="absoluteExpiration">所添加对象将到期并被从缓存中移除的时间。</param>
        /// <param name="slidingExpiration"> 最后一次访问所添加对象时与该对象到期时之间的时间间隔。 如果该值等效于 20 分钟，则对象在最后一次被访问 20 分钟之后将到期并从缓存中移除。   如果使用绝对到期，则 slidingExpiration 参数必须为 System.Web.Caching.Cache.NoSlidingExpiration。</param>
        /// <param name="priority">对象的相对成本，由 System.Web.Caching.CacheItemPriority 枚举表示。 缓存在退出对象时使用该值；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。</param>
        /// <param name="callback"> 在从缓存中移除对象时所调用的委托（如果提供）。 当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Add<T>(IDictionary<string, T> items, CacheDependency dependencies, DateTime absoluteExpiration,
            TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            WebCache.Add<T>(items, dependencies, absoluteExpiration, slidingExpiration, priority, callback);
        }

        /// <summary>
        /// 添加K/V集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="items"> 键/值对的泛型集合。</param>
        /// <param name="dependencies">该项的文件依赖项或缓存键依赖项。 当任何依赖项更改时，该对象即无效，并从缓存中移除。 如果没有依赖项，则此参数包含 null。</param>
        /// <param name="absoluteExpiration">所添加对象将到期并被从缓存中移除的时间。</param>
        /// <param name="slidingExpiration"> 最后一次访问所添加对象时与该对象到期时之间的时间间隔。 如果该值等效于 20 分钟，则对象在最后一次被访问 20 分钟之后将到期并从缓存中移除。   如果使用绝对到期，则 slidingExpiration 参数必须为 System.Web.Caching.Cache.NoSlidingExpiration。</param>
        /// <param name="priority">对象的相对成本，由 System.Web.Caching.CacheItemPriority 枚举表示。 缓存在退出对象时使用该值；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。</param>
        /// <param name="callback"> 在从缓存中移除对象时所调用的委托（如果提供）。 当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration,
            TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            WebCache.Add(GetKeySuffix(key), value, dependencies, absoluteExpiration, slidingExpiration, priority, callback);
        }

        /// <summary>
        /// 获取K/V键对集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="keys">键集合</param>
        /// <returns>返回键/值对的泛型集合</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public IDictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            var enumerable = keys as string[] ?? keys.ToArray();
            enumerable.ForEach(key => key = GetKeySuffix(key));
            return WebCache.Get<T>(enumerable);
        }

        /// <summary>
        /// 异步获取K/V键对集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="keys">键集合</param>
        /// <returns>返回键/值对的泛型集合</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public async Task<IDictionary<string, T>> GetAsync<T>(IEnumerable<string> keys)
        {
            var enumerable = keys as string[] ?? keys.ToArray();
            enumerable.ForEach(key => key = GetKeySuffix(key));
            var result = await Task.Run(() => WebCache.Get<T>(enumerable));
            return result;
        }

        /// <summary>
        /// 根据KEY键获取返回对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对象</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public object Get(string key)
        {
            return WebCache.Get(GetKeySuffix(key));
        }


        /// <summary>
        /// 根据KEY异步获取返回对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<object> GetAsync(string key)
        {
            var result = await Task.Run(() => WebCache.Get(GetKeySuffix(key)));
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
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public T Get<T>(string key)
        {
            return WebCache.Get<T>(GetKeySuffix(key));
        }

        /// <summary>
        /// 根据KEY键异步获取返回指定的类型对象
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="key">KEY值</param>
        /// <returns>需要返回的对象</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public async Task<T> GetAsync<T>(string key)
        {
            var result = await Task.Run(() => WebCache.Get<T>(GetKeySuffix(key)));
            return result;
        }

        /// <summary>
        /// 根据KEY键获取转化成指定的对象，指示获取转化是否成功的返回值
        /// </summary>
        /// <param name="key">KEY键</param>
        /// <param name="obj">需要转化返回的对象</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public bool GetCacheTryParse(string key, out object obj)
        {
            return WebCache.GetCacheTryParse(GetKeySuffix(key), out obj);
        }

        /// <summary>
        /// 根据KEY键删除缓存
        /// </summary>
        /// <param name="key">KEY键</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void Remove(string key)
        {
            WebCache.Remove(key);
        }

        /// <summary>
        /// 根据KEY异步删除缓存
        /// </summary>
        /// <param name="key">KEY键</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void RemoveAsync(string key)
        {
            Task.Run(() => WebCache.Remove(GetKeySuffix(key)));
        }

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void RemoveAll()
        {
            WebCache.RemoveAll();
        }

        /// <summary>
        /// 根据正则表达式模糊匹配删除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/5</para>
        /// </remarks>
        public void RemoveByPattern(string pattern)
        {
            WebCache.RemoveByPattern(pattern);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 默认缓存失效时间
        /// </summary>
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

        /// <summary>
        /// KEY前缀
        /// </summary>
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

        #endregion

        #region 私有变量
        private string GetKeySuffix(string key)
        {
            return string.IsNullOrEmpty(KeySuffix) ? key : string.Format("_{0}_{1}", KeySuffix, key);
        }
        #endregion
    }
}