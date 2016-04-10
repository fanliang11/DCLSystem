using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCLSystem.Core.Caching.HashAlgorithms;
using DCLSystem.Core.Caching.RedisCache;
using ServiceStack.Redis;

namespace DCLSystem.Core.Caching.Transactions
{
   [IdentifyCache(name: CacheTargetType.Redis)]
   public  class RedisTransactionScope:IDisposable
   {
       #region 字段
       private readonly Lazy<ICacheProvider> _context;
       private readonly RedisClient redisClient;
        private readonly string key;
       #endregion

        public RedisTransactionScope(string appName)
       {
          // _context = new Lazy<RedisContext>(() => CacheContainer.GetInstances<RedisContext>(appName));
       }

        private ConsistentHashNode GetRedisNode(string item)
        {
            //ConsistentHash<ConsistentHashNode> hash;
            //_context.Value.dicHash.TryGetValue(CacheTargetType.Redis.ToString(), out hash);
            //return hash != null ? hash.GetItemNode(item) : default(ConsistentHashNode);
            return null;
        } 

        public void Dispose()
        {
            redisClient.Remove(key);
        }
    }
}
