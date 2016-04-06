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
            var d = DateTime.Now;
            var list = new List<Task<string>>();
            for (var i = 0; i < 1000; i++)
            {
                var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");
              //  o.AddAsync("dddd", "gggg", 4444);
                list.Add(o.GetAsync<string>("dddd"));

            }
            var r = list.Select(p => p.Result).ToList();
            var t = (DateTime.Now - d).TotalMilliseconds;
            var o1 = CacheContainer.GetInstances<ICacheProvider>("testCache.Redis");
            for (var i = 0; i < 1000; i++)
            {
                var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");
                //  o.AddAsync("dddd","gggg",4444);
                list.Add(o.GetAsync<string>("dddd"));

            }
            var r1 = list.Select(p => p.Result).ToList();
            var o2 = CacheContainer.GetInstances<WebCacheProvider>(CacheTargetType.WebCache.ToString());
            o2.Add("dggh2", "111");
            o1.Add("3333", "33333", 4444);


            var b1 = o1.Get<string>("3333");
            var pool = CacheContainer.GetInstances<RedisContext>("ddlCache").DataContextPool;
            var pool1 = CacheContainer.GetInstances<RedisContext>("ddlCache").DataContextPool;
            var r3 = o2.GetAsync<string>("dggh2").Result;

        }
    }
}
