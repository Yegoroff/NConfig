using System;
using System.Configuration;
using System.Linq;
using System.Configuration.Internal;
using System.Reflection;
using System.Collections.Generic;

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
            IInternalConfigSystem originalConfiguration =  ReplaceConfigurationManager(factory, fileNames);

            //TODO: Add ability to restore original configuration if possible.

            // Web Part (10 level black magic starts here)
            var httpConfigurationSystem = new ReflectionAccessor(originalConfiguration.GetType());

            // Get original values.
            IConfigSystem configSystem = httpConfigurationSystem.GetField<IConfigSystem>("s_configSystem");

            NConfigSystemReplacement replacingSystem = new NConfigSystemReplacement(configSystem, factory, fileNames);

            // Substitute to decorated instances.
            httpConfigurationSystem.SetField("s_configSystem", replacingSystem);
            httpConfigurationSystem.SetField("s_configRoot", replacingSystem.Root);

            // Clear cache, so it will be refilled with new decorated records.
            var systemWebAss = httpConfigurationSystem.AccessedType.Assembly;
            var hostingEnviroment = new ReflectionAccessor(systemWebAss.GetType("System.Web.Hosting.HostingEnvironment"));
            string siteId = hostingEnviroment.GetProperty<string>("SiteID");
            string configPath = "dmachine/webroot/" + siteId;

            var httpRuntime = new ReflectionAccessor(systemWebAss.GetType("System.Web.HttpRuntime"));
            var cache = new ReflectionAccessor(httpRuntime.GetProperty("CacheInternal"));

            while (true)
            {
                cache.Execute("Remove", configPath);

                int index = configPath.LastIndexOf('/');
                if (index < 0)
                    break;
                configPath = configPath.Substring(0, index);
            }
        }


        public static void SubstituteClientConfigSystem(IConfigurationFactorty factory, IList<string> fileNames)
        {
            ReplaceConfigurationManager(factory, fileNames);
        }


        public static void RestoreInternalConfigSystem()
        {
            if (originalConfiguration != null)
            {
                var configManager = new ReflectionAccessor(typeof(ConfigurationManager));
                configManager.SetField("s_configSystem", originalConfiguration);
                originalConfiguration = null;
            }
        }


        private static IInternalConfigSystem ReplaceConfigurationManager(IConfigurationFactorty factory, IList<string> fileNames)
        {
            if (originalConfiguration != null)
                throw new InvalidOperationException("Web system default configuration already substituted.");

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));

            ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
            originalConfiguration = configManager.GetField<IInternalConfigSystem>("s_configSystem");

            NSystemReplacementConfiguration replacingConfigSytem = factory.CreateSystemReplacementConfiguration(originalConfiguration, fileNames);

            configManager.SetField("s_configSystem", replacingConfigSytem);

            return originalConfiguration;
        }

    }
}
