using System.Collections.Generic;
using System.Linq;

namespace NConfig
{
    public static class NConfigurator
    {
        private static readonly IConfigurationRepository repository = new ConfigurationRepository();
        private static readonly INConfigSettings settings = new NConfigSettings(repository);
        private static INConfiguration defaultConfig = new NMultifileConfiguration(repository, null);


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


        public static INConfiguration FromFile(string fileName)
        {
            return new NMultifileConfiguration(repository, new List<string> {settings.GetAliasedFileName(fileName), fileName});
        }

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


    }
}
