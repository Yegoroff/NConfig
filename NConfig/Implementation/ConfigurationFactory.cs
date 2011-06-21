using System.Collections.Generic;
using System.Configuration.Internal;

namespace NConfig
{
    internal class ConfigurationFactory : IConfigurationFactory
    {

        public ConfigurationFactory(IConfigurationRepository configurationRepository, INSectionMergerRegistry mergerRegistry)
        {
            ConfigurationRepository = configurationRepository;
            MergerRegistry = mergerRegistry;
        }


        #region IConfigurationFactorty Members

        public INSectionMergerRegistry MergerRegistry
        {
            get;
            private set;
        }

        public IConfigurationRepository ConfigurationRepository
        {
            get;
            private set;
        }


        public INConfiguration CreateConfiguration(IList<string> fileNames)
        {
            return new NMultifileConfiguration(ConfigurationRepository, MergerRegistry, fileNames);
        }

        public NSystemReplacementConfiguration CreateSystemReplacementConfiguration(IInternalConfigSystem originalConfiguration, IList<string> fileNames)
        {
            return new NSystemReplacementConfiguration(originalConfiguration, ConfigurationRepository, MergerRegistry, fileNames);
        }

        public INConfiguration CreateConfigRecordConfiguration(IInternalConfigRecord configRecord, IList<string> fileNames)
        {
            return new NConfigRecordConfiguration(configRecord, ConfigurationRepository, MergerRegistry, fileNames);
        }

        #endregion

    }


}
