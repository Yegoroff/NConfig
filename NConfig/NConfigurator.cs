using System.Collections.Generic;
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
        private static readonly IConfigurationFactory configurationFactory;
        private static readonly INSystemConfigurator systemConfigurator;


        static NConfigurator()
        {
            mergerRegistry = CreateSectionMerger();
            repository = CreateRepository();
            settings = new NConfigSettings(Repository);
            configurationFactory = new ConfigurationFactory(Repository, MergerRegistry);
            Default = ConfigurationFactory.CreateConfiguration(null);
            systemConfigurator = CreateSystemConfigurator();
        }


        /// <summary>
        /// Gets the current application's default configuration.
        /// </summary>
        /// <value>The current application's default configuration.</value>
        public static INConfiguration Default { get; private set; }


        /// <summary>
        ///     Provides access to configuration stored in the specified client configuration file.
        /// </summary>
        /// <param name="fileName">The path of the configuration file.</param>
        public static INConfiguration UsingFile(string fileName)
        {
            return ConfigurationFactory.CreateConfiguration(new List<string> {settings.GetAliasedFileName(fileName), fileName});
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

            return ConfigurationFactory.CreateConfiguration(configNames);
        }

        /// <summary>
        /// Restores the system default configuration.
        /// </summary>
        public static void RestoreSystemDefaults()
        {
            SystemConfigurator.RestoreSystemConfiguration();
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

        internal static IConfigurationFactory ConfigurationFactory
        {
            get
            {
                return configurationFactory;
            }
        }

        internal static INConfigSettings Settings
        {
            get
            {
                return settings;
            }
        }

        internal static  INSystemConfigurator SystemConfigurator
        {
            get
            {
                return systemConfigurator;
            }
        }


        internal static void SetDefaultConfiguration(INConfiguration config)
        {
            Default = config;
        }

        internal static void SubstituteSystemConfiguration(INConfiguration config)
        {
            SetDefaultConfiguration(config);
            SystemConfigurator.SubstituteSystemConfiguration(NConfigurator.ConfigurationFactory, config.FileNames);            
        }


        private static IConfigurationRepository CreateRepository()
        {
            if (NConfigSettings.DetectIsWeb())
                return new ConfigurationRepositoryWeb();

            return new ConfigurationRepository();
        }

        private static NSectionMergerRegistry CreateSectionMerger()
        {
            var result = new NSectionMergerRegistry();
            result.AddMerger(typeof(AppSettingsSection), new AppSettingsMerger());
            result.AddMerger(typeof(ConnectionStringsSection), new ConnectionStringsMerger());
            return result;
        }

        private static INSystemConfigurator CreateSystemConfigurator()
        {
            if (NConfigSettings.DetectIsWeb())
                return new NWebSystemConfigurator();

            return new NSystemConfigurator();

        }

    }
}

