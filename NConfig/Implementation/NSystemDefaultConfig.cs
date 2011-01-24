using System;
using System.Configuration;
using System.Linq;
using System.Configuration.Internal;
using System.Reflection;
using System.Collections.Generic;

namespace NConfig
{
    internal sealed class NSystemDefaultConfig : NMultifileConfiguration, IInternalConfigSystem
    {
        private static IInternalConfigSystem originalConfiguration;

        private ConnectionStringsSection connectionsSection;

        // Get IConfigSystem form ConfigurationManager  also we should affect HttpConfigurationSystem .s_ConfigSystem
        public static void SubstituteInternalConfigSystem(NSystemDefaultConfig newConfigSystem)
        {
            var fieldInfo = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);

            if (originalConfiguration == null)
            {
                ConfigurationManager.GetSection("appSettings"); // This will init COnfiguration manager internal config system.
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



        public NSystemDefaultConfig(IConfigurationRepository repository, IList<string> fileNames) :
            base (repository, fileNames) { }


        protected override object GetAppWebSection(string sectionName)
        {
            return originalConfiguration.GetSection(sectionName);
        }


        public ConnectionStringsSection ConnectionsSection
        {
            get
            {
                if (connectionsSection == null) 
                {
                    connectionsSection = new ConnectionStringsSection();
                    foreach(ConnectionStringSettings set in ConnectionStrings)
                        connectionsSection.ConnectionStrings.Add(set);
                }
                return connectionsSection;                
            }
        }


        #region IInternalConfigSystem Members

        object IInternalConfigSystem.GetSection(string configKey)
        {
            if (configKey == "connectionStrings")
                return ConnectionsSection;
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
