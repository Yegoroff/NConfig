using System.Collections.Generic;
using System.Linq;

namespace NConfig
{
    /// <summary>
    ///   Provides access to configuration files for client applications.
    /// </summary>
    public static class NConfigurator
    {
        private static readonly IConfigurationRepository repository = CreateRepository();
        private static readonly INConfigSettings settings = new NConfigSettings(repository);
        private static INConfiguration defaultConfig = new NMultifileConfiguration(repository, null);


        /// <summary>
        /// Gets the current application's default configuration.
        /// </summary>
        /// <value>The current application's default configuration.</value>
        public static INConfiguration Default
        {
            get
            {
                return defaultConfig;
            }
            internal set
            {
                defaultConfig = value;
            }
        }


        /// <summary>
        ///     Provides access to configuration stored in the specified client configuration file.
        /// </summary>
        /// <param name="fileName">The path of the configuration file.</param>
        public static INConfiguration FromFile(string fileName)
        {
            return new NMultifileConfiguration(repository, new List<string> {settings.GetAliasedFileName(fileName), fileName});
        }

        /// <summary>
        ///     Provides access to configuration stored in the specified client configuration files.
        /// </summary>
        /// <param name="fileName">The array of path of the configuration files.</param>
        public static INConfiguration FromFiles(params string[] fileNames)
        {
            List<string> configNames = new List<string>(fileNames.Length * 2);
            foreach (string name in fileNames)
            {
                configNames.Add(settings.GetAliasedFileName(name));
                configNames.Add(name);
            }

            return new NMultifileConfiguration(repository, configNames);
        }

        /// <summary>
        /// Restores the system default configuration.
        /// </summary>
        public static void RestoreSystemDefaults()
        {
            NSystemDefaultConfig.RestoreInternalConfigSystem();
        }



        internal static IConfigurationRepository Repository
        {
            get
            {
                return repository;
            }
        }

        internal static INConfigSettings Settings
        {
            get
            {
                return settings;
            }
        }


        private static IConfigurationRepository CreateRepository()
        {
            if (NConfigSettings.DetectIsWeb())
                return new ConfigurationRepositoryWeb();
            else
                return new ConfigurationRepository();
        }

    }
}
