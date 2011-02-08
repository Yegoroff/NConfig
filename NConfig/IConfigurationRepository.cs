using System.Configuration;

namespace NConfig
{
    internal interface IConfigurationRepository
    {
        /// <summary>
        /// Returns the Configuration object that corresponds to passed configuration file.
        /// </summary>
        /// <param name="fileName">Name of the configuration file.</param>
        /// <returns>The Configuration instance that provides access to configuration.</returns>
        Configuration GetFileConfiguration(string fileName);
    }
}
