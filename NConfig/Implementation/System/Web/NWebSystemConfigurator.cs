using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;

namespace NConfig
{
    /// <summary>
    /// Encapsulates forbidden magic that allows to smoothly use NConfig and System.Web.Configuration
    /// </summary>
    internal class NWebSystemConfigurator : NSystemConfigurator
    {

        public override void SubstituteSystemConfiguration(IConfigurationFactory factory, IList<string> fileNames)
        {
            if (OriginalConfiguration != null)
                throw new InvalidOperationException("Web system default configuration already substituted.");

            IInternalConfigSystem originalConfigSystem = SubstituteConfigurationSystem(factory, fileNames);

            // Web Part (10 level black magic starts here)
            var httpConfigurationSystem = new ReflectionAccessor(originalConfigSystem.GetType());

            // Get original values.
            var configSystem = httpConfigurationSystem.GetField<IConfigSystem>("s_configSystem");

            var replacingSystem = new NConfigSystemReplacement(configSystem, factory, fileNames);

            // Substitute to decorated instances.
            httpConfigurationSystem.SetField("s_configSystem", replacingSystem);
            httpConfigurationSystem.SetField("s_configRoot", replacingSystem.Root);

            // Refill system cache with new decorated records.
            var systemWebAss = httpConfigurationSystem.AccessedType.Assembly;
            var hostingEnviroment = new ReflectionAccessor(systemWebAss.GetType("System.Web.Hosting.HostingEnvironment"));
            string siteId = hostingEnviroment.GetProperty<string>("SiteID");
            string configPath = "dmachine/webroot/" + siteId;

            var httpRuntime = new ReflectionAccessor(systemWebAss.GetType("System.Web.HttpRuntime"));
            object cacheInternal = httpRuntime.GetProperty("CacheInternal");

            var rootReplacement = replacingSystem.Root as NConfigRootReplacement;

            // In case of single CPU internal caching uses CacheSingle.
            if (cacheInternal.GetType().Name == "CacheSingle")
                UpdateCacheSingle(rootReplacement, cacheInternal, configPath);
            else
                UpdateCacheMultiple(rootReplacement, configPath, cacheInternal);

            OriginalConfiguration = originalConfigSystem;
        }

        private static void UpdateCacheMultiple(NConfigRootReplacement rootReplacement, string configPath, object cacheInternal)
        {
            var cacheMultiple = new ReflectionAccessor(cacheInternal);
			var caches = cacheMultiple.GetField("_caches") as IEnumerable ?? Enumerable.Empty<object>();

			foreach (var cache in caches)
            {
                // Caches stored in array ala hash, so there are could be gaps.
                if (cache == null)
                    continue;

                UpdateCacheSingle(rootReplacement, cache, configPath);
            }

			//Private field from System.Web.Caching.CacheMultiple used to be called _caches but can also be called _cachesRefs
			var cacheRefs = cacheMultiple.GetField("_cachesRefs") as IEnumerable ?? Enumerable.Empty<object>();
			foreach (var cacheRef in cacheRefs)
			{
				if (cacheRef == null)
					continue;

				var cacheAcessor = new ReflectionAccessor(cacheRef);
				var cache = cacheAcessor.GetProperty("Target");

				if (cache != null)
					UpdateCacheSingle(rootReplacement, cache, configPath);

			}
		}

        private static void UpdateCacheSingle(NConfigRootReplacement rootReplacement, object cache, string configPath)
        {
            var cacheAcessor = new ReflectionAccessor(cache);
            lock (cacheAcessor.GetField("_lock"))
            {
                var entries = cacheAcessor.GetField("_entries") as IEnumerable ?? Enumerable.Empty<object>();

                // Get all site specific configuration records keys for internal cache.

                // cache "entries" is a HashTable, so just iterate through
                foreach (DictionaryEntry entry in entries)
                {
                    var keyAccessor = new ReflectionAccessor(entry.Key);

                    // Only configuration cache entries replaced.
                    if (!keyAccessor.GetProperty("Key").ToString().StartsWith(configPath))
                        continue;

                    // Key and Value is the same object in the configuration cache entry.
                    var entryValueAccesor = new ReflectionAccessor(keyAccessor.GetField("_value"));
                    var runtimeConfigAccessor = new ReflectionAccessor(entryValueAccesor.GetField("_runtimeConfig"));

                    IInternalConfigRecord replacingRecord =
                        rootReplacement.CreateConfigRecord(runtimeConfigAccessor.GetField<IInternalConfigRecord>("_configRecord"));
                    runtimeConfigAccessor.SetField("_configRecord", replacingRecord);
                    runtimeConfigAccessor.SetField("_runtimeConfigLKG", null);
                }
            }
        }


        public override void RestoreSystemConfiguration()
        {
            //TODO: Add ability to restore original configuration if possible.
            throw new NotSupportedException("Restoring Web Configuration not supported.");
        }

    }
}
