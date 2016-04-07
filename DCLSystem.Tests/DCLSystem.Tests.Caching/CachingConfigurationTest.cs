using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using DCLSystem.Core.Caching.NetCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DCLSystem.Core.Caching;
using DCLSystem.Core.Caching.RedisCache;

namespace DCLSystem.Tests.Caching
{
    [TestClass]
    public class CachingConfigurationTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var d = DateTime.Now;
            var list = new List<Task<string>>();
            var redisO = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");
            for (var i = 0; i < 5000; i++)
            {
                //   var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");
                redisO.Add("dddd", "55", 333);
                //    list.Add(o.GetAsync<string>("dddd"));

            }
            //     var r = list.Select(p => p.Result).ToList();
            var o1 = CacheContainer.GetInstances<ICacheProvider>("testCache.Redis");

            var d = DateTime.Now;
            for (var i = 0; i < 1000; i++)
            {


                var g = redisO.Get<string>("dddd");
                // list.Add(o22.GetAsync<string>("dddd"));
            }
            //var r = list.Select(p => p.Result).ToList();
            // var r1 = list.Select(p => p.Result).ToList();
            var t = (DateTime.Now - d).TotalMilliseconds;
            var webCacheO = CacheContainer.GetInstances<WebCacheProvider>(CacheTargetType.WebCache.ToString());
            webCacheO.Add("dggh2", "111");
            o1.Add("3333", "33333", 4444);

            var list2 = new List<Task<object>>();
            var couchBase = CacheContainer.GetInstances<ICacheProvider>("ddlCache.CouchBase");
            var couchtime = DateTime.Now;
            for (var i = 0; i < 1000; i++)
            {
                var couchBase1 = CacheContainer.GetInstances<ICacheProvider>("ddlCache.CouchBase");
                var g = couchBase1.Get<string>("dg1");
            }
            var couchbaseTime = (DateTime.Now - couchtime).TotalMilliseconds;
            var b1 = o1.Get<string>("3333");
            var pool = CacheContainer.GetInstances<RedisContext>("ddlCache").DataContextPool;
            var pool1 = CacheContainer.GetInstances<RedisContext>("ddlCache").DataContextPool;
            var r3 = webCacheO.GetAsync<string>("dggh2").Result;

        }
    }
}
