using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLSystem.Core.Caching.Configurations
{
    public sealed class CacheWrapperSection : ConfigurationSection
    {
        #region 字段
        private const string ProviderKey = "providers";
        private const string BindingKey = "bindings";
        #endregion

        #region 属性
        [ConfigurationProperty(ProviderKey,IsRequired = false)]
        public ProviderCollection Providers
        {
            get { return (ProviderCollection)base[ProviderKey]; }
        }

        [ConfigurationProperty(BindingKey, IsRequired = false)]
        public BindingsCollection Bindings
        {
            get { return (BindingsCollection)base[BindingKey]; }
        }
        #endregion
    }
}
