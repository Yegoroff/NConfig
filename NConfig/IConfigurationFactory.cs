using System.Configuration;
using System.Collections.Generic;
using System.Configuration.Internal;

namespace NConfig
{
    internal interface IConfigurationFactorty
    {
        INSectionMergerRegistry MergerRegistry { get; }

        IConfigurationRepository ConfigurationRepository { get; }


        INConfiguration CreateConfiguration(IList<string> fileNames);

        NSystemReplacementConfiguration CreateSystemReplacementConfiguration(IInternalConfigSystem originalConfiguration, IList<string> fileNames);

        INConfiguration CreateConfigRecordConfiguration(IInternalConfigRecord configRecord, IList<string> fileNames);
    }


}
