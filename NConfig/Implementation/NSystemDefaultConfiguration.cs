using System;
using System.Configuration;
using System.Linq;
using System.Configuration.Internal;
using System.Reflection;
using System.Collections.Generic;

namespace NConfig
{
    internal sealed class NSystemDefaultConfiguration : NMultifileConfiguration, IInternalConfigSystem
    {
        private static IInternalConfigSystem originalConfiguration;


        private ConnectionStringsSection connectionsSection;


        // Get IConfigSystem form ConfigurationManager also we should affect HttpConfigurationSystem.s_ConfigSystem
        public static void SubstituteInternalConfigSystem(NSystemDefaultConfiguration newConfigSystem)
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



        public NSystemDefaultConfiguration(IConfigurationRepository repository, INSectionMergerRegistry mergerRegistry, IList<string> fileNames) :
            base (repository, mergerRegistry, fileNames) { }


        protected override object GetAppWebSection(string sectionName)
        {
            return originalConfiguration.GetSection(sectionName);
        }


        /// <summary>
        /// Gets the connection strings section.
        /// Used to provide caching of connection strings.
        /// </summary>
        /// <value>The connection strings section.</value>
        public ConnectionStringsSection ConnectionStringsSection
        {
            get
            {
                if (connectionsSection == null) 
                    connectionsSection = GetSection("connectionStrings") as ConnectionStringsSection;
                return connectionsSection;                
            }
        }


        #region IInternalConfigSystem Members

        object IInternalConfigSystem.GetSection(string configKey)
        {
            if (configKey == "connectionStrings")
                return ConnectionStringsSection;

            if (configKey == "appSettings")
                return this.AppSettings;

            return base.GetSection(configKey);
        }

        void IInternalConfigSystem.RefreshConfig(string sectionName)
        {
            originalConfiguration.RefreshConfig(sectionName);
        }

        bool IInternalConfigSystem.SupportsUserConfig
        {
            get 
            {
                return originalConfiguration.SupportsUserConfig;
            }
        }

        #endregion
    }
}
