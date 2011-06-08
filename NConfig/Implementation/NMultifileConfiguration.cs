using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace NConfig
{
    internal class NMultifileConfiguration : INConfiguration
    {
        private readonly IConfigurationRepository repository;
        private readonly INSectionMergerRegistry mergerRegistry;
        private readonly IList<string> fileNames;

        // Property memorization.
        private ConnectionStringSettingsCollection connectionStrings;
        private NameValueCollection appSettings;


        internal NMultifileConfiguration(IConfigurationRepository repository, 
            INSectionMergerRegistry mergerRegistry, IList<string> fileNames)
        {
            this.repository = repository;
            this.mergerRegistry = mergerRegistry;
            this.fileNames = fileNames ?? new List<string>();
        }


        public IConfigurationRepository Repository
        {
            get { return repository; }
        }

        public INSectionMergerRegistry MergerRegistry
        {
            get { return mergerRegistry; }
        }



        protected virtual object GetAppWebSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }

        protected ConfigurationSection GetDefaultSection(string sectionName)
        {
            object section = GetAppWebSection(sectionName);

            if (section != null)
            {
                // AppSettingsSection doesn't returned from ConfirationManager, instead returned NameValueCollection.
                if (sectionName.Equals("appSettings", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    AppSettingsSection appSection = new AppSettingsSection();
                    var appSettings = section as NameValueCollection;
                    if (appSettings != null)
                        for (int i = 0; i < appSettings.Count; i++)
                            appSection.Settings.Add(appSettings.GetKey(i), appSettings[i]);
                    return appSection;
                }
            }

            return section as ConfigurationSection;
        }

        private ConfigurationSection GetFileSection(string fileName, string sectionName)
        {
            Configuration config = Repository.GetFileConfiguration(fileName);
            if (config != null)
                return config.GetSection(sectionName);

            return null;           
        }



        private ConnectionStringSettingsCollection GetConnectionStrings()
        {
            var result = new ConnectionStringSettingsCollection();
            ConnectionStringsSection section = GetSection("connectionStrings") as ConnectionStringsSection;
            foreach (ConnectionStringSettings settings in section.ConnectionStrings)
                result.Add(settings);
            return result;
        }

        private NameValueCollection GetAppSettings()
        {
            var result = new NameValueCollection();
            AppSettingsSection appSection = GetSection("appSettings") as AppSettingsSection;
            if (appSection != null)
                foreach (KeyValueConfigurationElement element in appSection.Settings)
                    result.Add(element.Key, element.Value);

            return result;
        }


        #region INConfiguration Members

        public IList<string> FileNames
        {
            get { return fileNames; }
        }

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
            List<ConfigurationSection> sections = new List<ConfigurationSection>();
            
            // Read section from custom configuration files.
            ConfigurationSection section;
            foreach (string fileName in FileNames) // The order of files shoud be from most Important to lower
            {
                section = GetFileSection(fileName, sectionName);
                if (section != null && section.ElementInformation.IsPresent) // filter non-required sections.
                    sections.Add(section);
            }
           
            // Read Section form Configuration Manager.
            section = GetDefaultSection(sectionName);
            if (section != null) // Add not presented sections too, because of non required sections should be returned.
                sections.Add(section);
            
            // Merge collected sections.
            if (sections.Count > 1) {
                NSectionMerger merger = MergerRegistry.GetMerger(sections[0].GetType());
                return merger.Merge(sections);
            }

            if (sections.Count == 1)
                return sections[0];
            return null;
        }

        public virtual object GetSectionUntyped(string sectionName)
        {
            if (sectionName == "appSettings")
                return AppSettings;

            // Try to return ConfigurationSection object
            object res = GetSection(sectionName);
            if (res != null)
                return res;

            // Return any object returned by Default configuration system.
            return GetAppWebSection(sectionName);
        }

        //TODO: Provide Configuration group section union among config files.
        public ConfigurationSectionGroup GetSectionGroup(string groupName)
        {
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
