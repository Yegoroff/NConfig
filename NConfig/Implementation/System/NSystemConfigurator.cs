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


        public static void SubstituteWebConfigSystem(INConfiguration newConfigSystem)
        {
    //TODO: REAFACTOR fiels accessors, and other stuff (to proxy classes ?)

            // Common Part

            var fieldInfo = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);

            if (originalConfiguration == null)
            {
                ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
                originalConfiguration = fieldInfo.GetValue(null) as IInternalConfigSystem;

    //TODO: REFACTOR  add ability to restore.
                // Web Part (10 level black magic starts here)
                Type configSysType = originalConfiguration.GetType(); // In web this should be System.Web.Configuration.HttpConfigurationSystem

                var configSystemField = configSysType.GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
                var configRootField = configSysType.GetField("s_configRoot", BindingFlags.NonPublic | BindingFlags.Static);


                // Get original values.
                IConfigSystem configSystem = configSystemField.GetValue(null) as IConfigSystem;
                NConfigSystem decoratedSystem = new NConfigSystem(configSystem);

                // Substitute to decorated instances.
                configSystemField.SetValue(null, decoratedSystem);
                configRootField.SetValue(null, decoratedSystem.Root);


                // Clear cache so it will filled with new decorated instances 
                // config path = WebConfigurationHost.RootWebConfigPathAndDefaultSiteID  -- see who use this maybe access wuot reflection.
                // HttpRuntime.CacheInternal.Remove("d" + configPath)
            }

            NSystemDefaultConfiguration decoratedConfigSytem = 
                new NSystemDefaultConfiguration(originalConfiguration, NConfigurator.Repository, NConfigurator.MergerRegistry, newConfigSystem.FileNames);

            fieldInfo.SetValue(null, decoratedConfigSytem);
        }

        public static void SubstituteClientConfigSystem(INConfiguration newConfigSystem)
        {

            var fieldInfo = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);

            if (originalConfiguration == null)
            {
                ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
                originalConfiguration = fieldInfo.GetValue(null) as IInternalConfigSystem;
            }

            fieldInfo.SetValue(null, newConfigSystem);
        }

        public static void RestoreInternalConfigSystem()
        {
            if (originalConfiguration != null)
            {
                var fieldInfo = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
                fieldInfo.SetValue(null, originalConfiguration);
                originalConfiguration = null;
            }
        }

    }
}
