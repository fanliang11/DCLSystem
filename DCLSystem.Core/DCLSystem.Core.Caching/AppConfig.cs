using System.Globalization;
using System.Reflection;
using DCLSystem.Core.Caching.Configurations;
using DCLSystem.Core.Caching.DependencyResolution;
using DCLSystem.Core.Caching.HashAlgorithms;
using DCLSystem.Core.Caching.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLSystem.Core.Caching
{
    internal sealed class AppConfig
    {
        #region 字段

        private const string CacheSectionName = "cachingProvider";
        private readonly CacheWrapperSection _cacheWrapperSetting;
        private static readonly AppConfig _defaultInstance = new AppConfig();

        #endregion

        #region 构造函数

        public AppConfig(Configuration configuration)
            : this(
                configuration.AppSettings.Settings,
                (CacheWrapperSection) configuration.GetSection(CacheSectionName))
        {
            DebugCheck.NotNull(configuration);
        }



        private AppConfig()
            : this(
                Convert(ConfigurationManager.AppSettings),
                (CacheWrapperSection) ConfigurationManager.GetSection(CacheSectionName))
        {
        }


        private AppConfig(
            KeyValueConfigurationCollection appSettings,
            CacheWrapperSection cacheWrapperSetting
            )
        {
            ServiceResolver.Current.Register(null, Activator.CreateInstance(typeof (HashAlgorithm), new object[] {}));
            _cacheWrapperSetting = cacheWrapperSetting ?? new CacheWrapperSection();
            RegisterInstance();
            InitSettingMethod();
            var bings = _cacheWrapperSetting
                .Bindings
                .OfType<BindingCollection>().ToList().Select(p => p.GetTypedPropertyValues()).ToList();
        }

        #endregion

        internal static AppConfig DefaultInstance
        {
            get { return _defaultInstance; }
        }

        public T GetContextInstance<T>() where T : class
        {
            var context = ServiceResolver.Current.GetService<T>(typeof (T));
            return context;
        }

        public T GetContextInstance<T>(string name) where T : class
        {
            DebugCheck.NotEmpty(name);
            var context = ServiceResolver.Current.GetService<T>(name);
            return context;
        } 

        #region 私有方法

        private static KeyValueConfigurationCollection Convert(NameValueCollection collection)
        {
            var settings = new KeyValueConfigurationCollection();
            foreach (var key in collection.AllKeys)
            {
                settings.Add(key, ConfigurationManager.AppSettings[key]);
            }
            return settings;
        }

        private void RegisterInstance()
        {
            var bingingSettings = _cacheWrapperSetting.Bindings.OfType<BindingCollection>();

            try
            {
                var types = this.GetType().Assembly.GetTypes().Where(p => p.GetInterface("ICacheProvider") != null);
                foreach (var t in types)
                {
                    foreach (var setting in bingingSettings)
                    {
                        var properties = setting.GetTypedPropertyValues()
                            .OfType<PropertyElement>();
                        var propertyElements = properties as PropertyElement[] ?? properties.ToArray();
                        var args = propertyElements.Select(p => p.GetTypedPropertyValue())
                            .ToArray();
                        var maps =
                            propertyElements.Select(p => p.GetTypedParameterValues().OfType<MapCollection>())
                                .FirstOrDefault(p => p.Any());
                        var type = setting.GetFactoryType();

                          if(ServiceResolver.Current.GetService(type,setting.IdName)==null)
                        ServiceResolver.Current.Register(setting.IdName, Activator.CreateInstance(type, args));
                        if (maps == null) continue;
                        var mapCollections = maps as MapCollection[] ?? maps.ToArray();
                        if (!mapCollections.Any()) continue;

                        foreach (
                            var mapsetting in
                                mapCollections.Where(mapsetting => t.Name.StartsWith(mapsetting.Name, true,
                                    CultureInfo.InvariantCulture)))
                        {
                            ServiceResolver.Current.Register(string.Format("{0}.{1}", setting.IdName, mapsetting.Name),
                                Activator.CreateInstance(t, new object[] {setting.IdName}));
                        }
                    }
                    var attribute = t.GetCustomAttribute<IdentifyCacheAttribute>();
                    ServiceResolver.Current.Register(attribute.Name.ToString(),
                        Activator.CreateInstance(t));
                }
            }
            catch
            {

            }
        }

        private void InitSettingMethod()
        {
            var settings =
                _cacheWrapperSetting.Bindings.OfType<BindingCollection>()
                    .Where(p => !string.IsNullOrEmpty(p.InitMethodName));
            foreach (var setting in settings)
            {
                var bindingInstance =
                    ServiceResolver.Current.GetService(Type.GetType(setting.ClassName, throwOnError: true),
                        setting.IdName);
                bindingInstance.GetType()
                    .InvokeMember(setting.InitMethodName, System.Reflection.BindingFlags.InvokeMethod, null,
                        bindingInstance, new object[] {});
            }
        }

        #endregion
    }
}
