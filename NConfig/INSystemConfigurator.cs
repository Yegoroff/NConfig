using System.Collections.Generic;
namespace NConfig
{
    internal interface INSystemConfigurator
    {
        void SubstituteSystemConfiguration(IConfigurationFactory factory, IList<string> fileNames);
     
        void RestoreSystemConfiguration();
    }
}
