using System.Configuration;

namespace NConfig
{
    /// <summary>
    /// Provides Host names to Aliases mapping.
    /// </summary>
    public class HostMapSection : ConfigurationSection
    {

        /// <summary>
        /// Gets the Host Names to Aliases mappings.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public HostMappingConfigurationCollection Mappings
        {
            get { return (HostMappingConfigurationCollection)this[""]; }
        }

    }

}
