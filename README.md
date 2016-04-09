# DCLSystem
分布式缓存，实现一致性哈希算法，集成 couchbase,redis,webcache,membercache:

     var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Redis");

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");

couchbase:

    var o = CacheContainer.GetInstances<ICacheProvider>("ddlCache.Couchbase");

          o.Add("dddd","gggg",60);

          var b = o.Get<string>("dddd");
          
          value内容实现了GZIP压缩

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
web.config


 <configSections>
    <section name="cachingProvider" type="DCLSystem.Core.Caching.Configurations.CacheWrapperSection, DCLSystem.Core.Caching" requirePermission="false" />
  </configSections>
  <cachingProvider>
  
    <bindings>
    
      <binding id="ddlCache" class="DCLSystem.Core.Caching.RedisCache.RedisContext,DCLSystem.Core.Caching">
        <property name="appRuleFile" ref="rule"/>
        <property name="dataContextPool" value="ddls_sample">
          <map name="Redis">
            <property  value="127.0.0.1:6379::4"/>
          </map>
          <map name="WebCache"></map>
           <map name="CouchBase">
             <property  value="fanly:12345@192.168.0.1:6379::pools"/>
          </map>
        </property>
        <property name="defaultExpireTime" value="120"/>
       <property name="connectTimeout" value="120"/>
       <property name="minSize" value="1"/>
      <property name="maxSize" value="200"/>
      </binding>
         <binding id="testCache" class="DCLSystem.Core.Caching.RedisCache.RedisContext,DCLSystem.Core.Caching">
        <property name="appRuleFile" ref="rule"/>
        <property name="dataContextPool" value="ddls_sample">
          <map name="Redis">
            <property  value="127.0.0.1:6379::4"/>
          </map>
           <map name="CouchBase">
            <property  value="fanly:12345@192.168.0.1:6379"/>
          </map>
        </property>
        <property name="defaultExpireTime" value="120"/>
       <property name="connectTimeout" value="120"/>
       <property name="minSize" value="1"/>
      <property name="maxSize" value="50"/>
      </binding>
    </bindings>
  </cachingProvider>
