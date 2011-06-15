using System;
using System.Configuration;
using System.Linq;
using System.Configuration.Internal;
using System.Reflection;
using System.Collections.Generic;

namespace NConfig
{
    internal sealed class NSystemReplacementConfiguration : NMultifileConfiguration, IInternalConfigSystem
    {
        private readonly IInternalConfigSystem originalConfiguration;


        public NSystemReplacementConfiguration(IInternalConfigSystem originalConfiguration, IConfigurationRepository repository, INSectionMergerRegistry mergerRegistry, IList<string> fileNames) :
            base(repository, mergerRegistry, fileNames)
        {
            this.originalConfiguration = originalConfiguration;
        }



        protected override object GetAppWebSection(string sectionName)
        {
            return originalConfiguration.GetSection(sectionName);
        }


        #region IInternalConfigSystem Members

        object IInternalConfigSystem.GetSection(string configKey)
        {
            return GetSectionUntyped(configKey);
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
