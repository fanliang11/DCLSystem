using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLSystem.Core.Caching
{
   public class CacheContainer
    {
       public static T GetInstances<T>(string name) where T:class
       {
           var appConfig = AppConfig.DefaultInstance;
          return appConfig.GetContextInstance<T>(name);
       }

       public static T GetInstances<T>()  where T:class
       {
           var appConfig = AppConfig.DefaultInstance;
           return appConfig.GetContextInstance<T>();
       }
    }
}
