using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;

namespace NConfig
{
    internal class NMultifileConfiguration : INConfiguration
    {
        private readonly IConfigurationRepository repository;
        private readonly IList<string> fileNames;

        private ConnectionStringSettingsCollection connectionStrings;
        private NameValueCollection appSettings;


        internal NMultifileConfiguration(IConfigurationRepository repository, IList<string> fileNames)
        {
            this.repository = repository;
            this.fileNames = fileNames ?? new List<string>();
        }


        public IConfigurationRepository Repository
        {
            get
            {
                return repository;
            }
        }


        protected virtual object GetAppWebSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }


        private ConfigurationSection GetFileSection(string fileName, string sectionName)
        {
            Configuration config = Repository.GetFileConfiguration(fileName);
            if (config != null)
                return config.GetSection(sectionName);

            return null;
        }


//TODO: Consider ISectionMerger { TConfigSection Merge(IEnumerable<TConfigSection>); }
        private ConnectionStringSettingsCollection GetConnectionStrings()
        {
            var connections = new Dictionary<string, ConnectionStringSettings>();

            ConnectionStringsSection section = GetAppWebSection("connectionStrings") as ConnectionStringsSection;
            foreach (ConnectionStringSettings settings in section.ConnectionStrings)
                connections[settings.Name] = settings;

            foreach (string fileName in FileNames.Reverse())
            {
                section = GetFileSection(fileName, "connectionStrings") as ConnectionStringsSection;
                if (section != null)
                    foreach (ConnectionStringSettings settings in section.ConnectionStrings)
                        connections[settings.Name] = settings;
            }

            var result = new ConnectionStringSettingsCollection();
            foreach (ConnectionStringSettings settings in connections.Values)
                result.Add(settings);

            return result;
        }

        private NameValueCollection GetAppSettings()
        {
            var settings = new Dictionary<string, string>();

            var appSettings = GetAppWebSection("appSettings") as NameValueCollection;
            if (appSettings != null)
                for (int i = 0; i < appSettings.Count; i++)
                    settings[appSettings.GetKey(i)] = appSettings[i];

            
            AppSettingsSection section;
            foreach (string fileName in FileNames.Reverse()) // The order of files shoud be from most Important to lower
            {
                section = GetFileSection(fileName, "appSettings") as AppSettingsSection;
                if (section != null)
                    foreach (KeyValueConfigurationElement element in section.Settings)
                        settings[element.Key] = element.Value;
            }

            var result = new NameValueCollection();
            foreach (var element in settings)
                result.Add(element.Key, element.Value);

            return result;
        }


        #region INConfiguration Members

        public IList<string> FileNames { get { return fileNames; } }

        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                if (connectionStrings == null)
                    connectionStrings = GetConnectionStrings();
                return connectionStrings;
            }
        }

        public NameValueCollection AppSettings
        {
            get
            {
                if (appSettings == null)
                    appSettings = GetAppSettings();
                return appSettings;
            }
        }


        public ConfigurationSection GetSection(string sectionName)
        {
            ConfigurationSection section;
            foreach (string fileName in FileNames) // The order of files shoud be from most Important to lower
            {
                section = GetFileSection(fileName, sectionName);
                if (section != null && section.ElementInformation.IsPresent == true)
                    return section;
            }
           
            return GetAppWebSection(sectionName) as ConfigurationSection;
        }

        public ConfigurationSectionGroup GetSectionGroup(string groupName)
        {

            // Reads configuration group from file prefixed with Alias., if file not exists then reads from "fileName" file

            ConfigurationSectionGroup group = null;
            foreach (string fileName in FileNames) // The order of files shoud be from most Important to lower
            {
                Configuration config = Repository.GetFileConfiguration(fileName);
                if (config != null)
                    group = config.GetSectionGroup(groupName);
                if (group != null)
                    return group;
            }

            return null;
        }

        #endregion

    }

}
