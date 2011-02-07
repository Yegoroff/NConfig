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
        private ConnectionStringsSection connectionsSection;
        private readonly IInternalConfigSystem originalConfiguration;


        public NSystemDefaultConfiguration(IInternalConfigSystem originalConfiguration, IConfigurationRepository repository, INSectionMergerRegistry mergerRegistry, IList<string> fileNames) :
            base(repository, mergerRegistry, fileNames)
        {
            this.originalConfiguration = originalConfiguration;
        }



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

            // Try to return ConfigurationSection object
            object res = base.GetSection(configKey);
            if (res != null)
                return res;

            // Return any object returned by Default configuration system.
            return GetAppWebSection(configKey);
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
