using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Internal;
using System.Linq;

namespace NConfig
{
    /// <summary>
    /// Incapsulates forbidden magic that allows to smoothly use NConfig and System.Configuration
    /// </summary>
    internal static class NSystemConfigurator
    {

        private static IInternalConfigSystem originalConfiguration;


        public static void SubstituteWebConfigSystem(IConfigurationFactorty factory, IList<string> fileNames)
        {
            IInternalConfigSystem originalConfigSystem = ReplaceConfigurationManager(factory, fileNames);

            //TODO: Add ability to restore original configuration if possible.

            // Web Part (10 level black magic starts here)
            var httpConfigurationSystem = new ReflectionAccessor(originalConfigSystem.GetType());

            // Get original values.
            var configSystem = httpConfigurationSystem.GetField<IConfigSystem>("s_configSystem");

            var replacingSystem = new NConfigSystemReplacement(configSystem, factory, fileNames);

            // Substitute to decorated instances.
            httpConfigurationSystem.SetField("s_configSystem", replacingSystem);
            httpConfigurationSystem.SetField("s_configRoot", replacingSystem.Root);

            // Clear cache, so it will be refilled with new decorated records.
            var systemWebAss = httpConfigurationSystem.AccessedType.Assembly;
            var hostingEnviroment = new ReflectionAccessor(systemWebAss.GetType("System.Web.Hosting.HostingEnvironment"));
            string siteId = hostingEnviroment.GetProperty<string>("SiteID");
            string configPath = "dmachine/webroot/" + siteId;

            var httpRuntime = new ReflectionAccessor(systemWebAss.GetType("System.Web.HttpRuntime"));
            var internalCache = new ReflectionAccessor(httpRuntime.GetProperty("CacheInternal"));
            var caches = internalCache.GetField("_caches") as IEnumerable ?? Enumerable.Empty<object>();

            // Get all site specific configuration records keys for internal cache.
            var configurationCacheEntries = new List<string>();
            foreach (var cache in caches)
            {
                if (cache != null)
                {
                    var cacheAcessor = new ReflectionAccessor(cache);
                    lock (cacheAcessor.GetField("_lock"))
                    {
                        var entries = cacheAcessor.GetField("_entries") as IEnumerable;
                        configurationCacheEntries.AddRange(
                            (from DictionaryEntry entry in entries
                            select new ReflectionAccessor(entry.Key).GetProperty("Key").ToString())
                            .Where(e => e.StartsWith(configPath))
                            );
                    }
                }
            }
            // Clear configuration records from cache.
            configurationCacheEntries.ForEach(cacheEntry => internalCache.Execute("Remove", cacheEntry));
        }


        public static void SubstituteClientConfigSystem(IConfigurationFactorty factory, IList<string> fileNames)
        {
            ReplaceConfigurationManager(factory, fileNames);
        }


        public static void RestoreInternalConfigSystem()
        {
            if (originalConfiguration == null)
                return;

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));
            configManager.SetField("s_configSystem", originalConfiguration);
            originalConfiguration = null;
        }


        private static IInternalConfigSystem ReplaceConfigurationManager(IConfigurationFactorty factory, IList<string> fileNames)
        {
            if (originalConfiguration != null)
                throw new InvalidOperationException("System default configuration already substituted.");

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));

            ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
            originalConfiguration = configManager.GetField<IInternalConfigSystem>("s_configSystem");

            NSystemReplacementConfiguration replacingConfigSytem = factory.CreateSystemReplacementConfiguration(originalConfiguration, fileNames);

            configManager.SetField("s_configSystem", replacingConfigSytem);

            return originalConfiguration;
        }

    }
}
