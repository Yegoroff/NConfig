using System.Configuration;
using System;

namespace NConfig
{
    public class HostMapSection : ConfigurationSection
    {

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public HostMappingConfigurationCollection Mappings
        {
            get { return (HostMappingConfigurationCollection)this[""]; }
        }

    }

}
