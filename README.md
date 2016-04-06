# DCLSystem
分布式缓存，实现一致性哈希算法，集成 couchbase,redis,webcache,membercache
redis:

     var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");

couchbase:

    var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Couchbase");

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");

MemberCache:

    var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.MemberCache");

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");
WebCache:

    var o = CacheContainer.GetInstances<WebCacheProvider>(CacheTargetType.WebCache.ToString());

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");
          
     var o = CacheContainer.GetInstances<ICacheProvider>("WebCache");

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");
          
支持异步添加，删除，获取

     var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");
          o.AddAsync("dddd", "gggg", 4444);
          o.GetAsync<string>("dddd").Result;
           o.RemoveAsync("dddd");
           
           DCLSystem拦截器集成
