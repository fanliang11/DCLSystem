using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLSystem.Core.Caching
{
    public  enum CacheTargetType
    {
        Redis,
        CouchBase,
        Memcached,
        WebCache,
    }
}
