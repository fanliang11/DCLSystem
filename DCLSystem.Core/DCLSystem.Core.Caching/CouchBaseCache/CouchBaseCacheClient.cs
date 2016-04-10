using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration;
using DCLSystem.Core.Caching.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Couchbase;
using DCLSystem.Core.Caching.Utilities;

namespace DCLSystem.Core.Caching.CouchBaseCache
{
    [IdentifyCache(name: CacheTargetType.CouchBase)]
    public class CouchBasecCacheClient : ICacheClient<CouchbaseClient>
    {
        private static readonly ConcurrentDictionary<string, CouchbaseClient> Clients =
   new ConcurrentDictionary<string, CouchbaseClient>();

        public CouchbaseClient GetClient(CacheEndpoint endpoint,int connectTimeout )
        {
            lock (Clients)
            {
                try
                {
                    var info = endpoint as CouchBaseEndpoint;
                    Check.NotNull(info, "endpoint");
                    var key = string.Format("{0}{1}{2}{3}{4}", info.Host, info.Port, info.BucketName, info.BucketPassword, info.Db);
                    if (!Clients.ContainsKey(key))
                    {
                        var clientConfiguration = new CouchbaseClientConfiguration();
                        var url = new Uri(string.Format("http://{0}:{1}/{2}", info.Host, info.Port, info.Db));
                        clientConfiguration.Bucket = info.BucketName;
                        clientConfiguration.BucketPassword = info.BucketPassword;
                        clientConfiguration.Urls.Add(url);
                        clientConfiguration.HttpRequestTimeout = TimeSpan.FromSeconds(connectTimeout);
                        clientConfiguration.SocketPool.MaxPoolSize = info.MaxSize;
                        clientConfiguration.SocketPool.MinPoolSize = info.MinSize;
                        var couchbaseClient = new CouchbaseClient(clientConfiguration);
                        Clients.GetOrAdd(key, couchbaseClient);
                        return couchbaseClient;
                    }
                    else
                    {
                        return Clients[key];
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }



    }
}
