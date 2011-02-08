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

        //TODO: Consider thread safety

        private static IInternalConfigSystem originalConfiguration;


        public static void SubstituteWebConfigSystem(INConfiguration newConfigSystem)
        {
            // Common Part
            if (originalConfiguration != null)
                throw new InvalidOperationException("Web system default configuration already substituted.");

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));

            ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
            originalConfiguration = configManager.GetField<IInternalConfigSystem>("s_configSystem");

            var decoratedConfigSytem =
                new NSystemDefaultConfiguration(originalConfiguration, NConfigurator.Repository, NConfigurator.MergerRegistry, newConfigSystem.FileNames);

            //TODO: Add ability to restore original configuration if possible.


            // Web Part (10 level black magic starts here)
            var httpConfigurationSystem = new ReflectionAccessor(originalConfiguration.GetType());

            // Get original values.
            IConfigSystem configSystem = httpConfigurationSystem.GetField<IConfigSystem>("s_configSystem");
            NConfigSystem decoratedSystem = new NConfigSystem(configSystem, decoratedConfigSytem);

            // Substitute to decorated instances.
            httpConfigurationSystem.SetField("s_configSystem", decoratedSystem);
            httpConfigurationSystem.SetField("s_configRoot", decoratedSystem.Root);

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

            configManager.SetField("s_configSystem", decoratedConfigSytem);
        }

        public static void SubstituteClientConfigSystem(INConfiguration newConfigSystem)
        {

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));

            if (originalConfiguration == null)
            {
                ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
                originalConfiguration = configManager.GetField<IInternalConfigSystem>("s_configSystem");
            }

            NSystemDefaultConfiguration decoratedConfigSytem =
                new NSystemDefaultConfiguration(originalConfiguration, NConfigurator.Repository, NConfigurator.MergerRegistry, newConfigSystem.FileNames);

            configManager.SetField("s_configSystem", decoratedConfigSytem);
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

    }
}
