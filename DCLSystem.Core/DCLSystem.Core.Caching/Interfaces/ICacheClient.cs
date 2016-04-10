using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCLSystem.Core.Caching.RedisCache;
using ServiceStack.Redis;

namespace DCLSystem.Core.Caching.Interfaces
{
    public interface  ICacheClient<T>
    {
        T GetClient(CacheEndpoint info, int connectTimeout);

    }
}
