using System.Collections.Generic;
using System.Linq;
using System;
using System.Configuration;

namespace NConfig
{
    /// <summary>
    ///   Provides access to configuration files for client applications.
    /// </summary>
    public static class NConfigurator
    {
        private static readonly INSectionMergerRegistry mergerRegistry;
        private static readonly IConfigurationRepository repository;
        private static readonly INConfigSettings settings;
        private static INConfiguration defaultConfig;


        static NConfigurator()
        {
            mergerRegistry = CreateSectionMerger();
            repository = CreateRepository();
            settings = new NConfigSettings(repository);
            defaultConfig = new NMultifileConfiguration(repository, mergerRegistry, null);
        }




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
        public static INConfiguration UsingFile(string fileName)
        {
            return new NMultifileConfiguration(Repository, MergerRegistry, new List<string> {settings.GetAliasedFileName(fileName), fileName});
        }

        /// <summary>
        ///     Provides access to configuration stored in the specified client configuration files.
        /// </summary>
        /// <param name="fileNames">The array of path of the configuration files.</param>
        public static INConfiguration UsingFiles(params string[] fileNames)
        {
            List<string> configNames = new List<string>(fileNames.Length * 2);
            foreach (string name in fileNames)
            {
                configNames.Add(settings.GetAliasedFileName(name));
                configNames.Add(name);
            }

            return new NMultifileConfiguration(Repository, MergerRegistry, configNames);
        }

        /// <summary>
        /// Restores the system default configuration.
        /// </summary>
        public static void RestoreSystemDefaults()
        {
            NSystemConfigurator.RestoreInternalConfigSystem();
        }

        /// <summary>
        /// Registers the section merger for speicfied section type.
        /// </summary>
        /// <param name="sectionType">Type of the section.</param>
        /// <param name="merger">The section merger instance.</param>
        public static void RegisterSectionMerger(Type sectionType, NSectionMerger merger)
        {
            MergerRegistry.AddMerger(sectionType, merger);
        }

        /// <summary>
        /// Registers the section merger for speicfied section type.
        /// </summary>
        /// <typeparam name="TSectionType">The type of the section.</typeparam>
        /// <param name="merger">The section merger instance.</param>
        public static void RegisterSectionMerger<TSectionType>(NSectionMerger merger)
        {
            RegisterSectionMerger(typeof(TSectionType), merger);
        }


        internal static IConfigurationRepository Repository
        {
            get
            {
                return repository;
            }
        }

        internal static INSectionMergerRegistry MergerRegistry
        {
            get
            {
                return mergerRegistry;
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

        private static NSectionMergerRegistry CreateSectionMerger()
        {
            var result = new NSectionMergerRegistry();
            result.AddMerger(typeof(AppSettingsSection), new AppSettingsMerger());
            result.AddMerger(typeof(ConnectionStringsSection), new ConnectionStringsMerger());
            return result;
        }
    }
}

