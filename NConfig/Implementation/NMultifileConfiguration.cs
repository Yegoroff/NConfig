using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace NConfig
{
    internal class NMultifileConfiguration : INConfiguration
    {
        private readonly IConfigurationRepository repository;
        private readonly INSectionMergerRegistry mergerRegistry;
        private readonly IList<string> fileNames;


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
                if (IsAppSettingsSection(sectionName))
                {
                    var appSection = new AppSettingsSection();
                    var appSettingsCollection = section as NameValueCollection;
                    if (appSettingsCollection != null)
                        for (int i = 0; i < appSettingsCollection.Count; i++)
                            appSection.Settings.Add(appSettingsCollection.GetKey(i), appSettingsCollection[i]);
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
            var section = GetSection("connectionStrings") as ConnectionStringsSection;
            if (section != null)
            {
                foreach (ConnectionStringSettings settings in section.ConnectionStrings)
                    result.Add(settings);
            }
            return result;
        }

        private NameValueCollection GetAppSettings()
        {
            var result = new NameValueCollection();
            var appSection = GetSection("appSettings") as AppSettingsSection;
            if (appSection != null)
                foreach (KeyValueConfigurationElement element in appSection.Settings)
                    result.Add(element.Key, element.Value);

            return result;
        }

        private static bool IsAppSettingsSection(string sectionName)
        {
            return sectionName.Equals("appSettings", System.StringComparison.InvariantCultureIgnoreCase);
        }

        // .Net Framework handles IConfigurationSectionHandler sections only for default configurations (app/web.config), not for custom loaded.
        // This is because IConfigurationSectionHandler is deprecated, but still alot of 3rd paty libraries use it. (log4net, nlog etc.)
        // These sections returned as DefaultSection from Configuration.GetSection so we need to handle such sections by our self.
        private object HandleIConfigurationSectionHandlerSection(DefaultSection section)
        {
            string sectionTypeName = section.SectionInformation.Type;
            var sectionType = Type.GetType(sectionTypeName);

            if (typeof (IConfigurationSectionHandler).IsAssignableFrom(sectionType))
            {
                // Create IConfigurationSectionHandler for this section.
                var handler = Activator.CreateInstance(sectionType) as IConfigurationSectionHandler;
                if (handler == null)
                    return null;

                var rawXml = section.SectionInformation.GetRawXml();
                var doc = new XmlDocument();
                doc.LoadXml(rawXml);

                // Invoke IConfigurationSectionHandler.Create method with read rawXlm as parameter.
                // We pass DocumentElement because some 3rd party libs expects XmlElement passed, not XmlNode as declared in Create signature.
                return handler.Create(null, null, doc.DocumentElement);
            }

            return null;
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
                // This property couldn't be caced, because of possible changes to app settings
                // and nested configuration files on web
                return GetConnectionStrings();
            }
        }

        public NameValueCollection AppSettings
        {
            get
            {
                // This property couldn't be caced, because of possible changes to app settings
                // and nested configuration files on webs
                return GetAppSettings();
            }
        }


        public ConfigurationSection GetSection(string sectionName)
        {
            var sections = new List<ConfigurationSection>();
            
            // Read section from custom configuration files.
            ConfigurationSection section;
            foreach (string fileName in FileNames) // The order of files should be from most Important to lower
            {
                section = GetFileSection(fileName, sectionName);
                // Filter out non-required sections (IsPresent == false) but leave DefaultSections, since them could represent IConfigurationSectionHandler sections.
                if (section != null && (section.ElementInformation.IsPresent || section is DefaultSection))
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
            if (IsAppSettingsSection(sectionName))
                return AppSettings;

            // Try to return ConfigurationSection object
            var res = GetSection(sectionName);

            // Handle IConfigurationSectionHandler sections.
            if (res is DefaultSection)
                return HandleIConfigurationSectionHandlerSection((DefaultSection)res);

            if (res != null)
                return res;

            // Return any object returned by Default configuration system.
            return GetAppWebSection(sectionName);
        }

        //TODO: Provide Configuration group section union among config files.
        public ConfigurationSectionGroup GetSectionGroup(string groupName)
        {
            ConfigurationSectionGroup group = null;
            foreach (string fileName in FileNames) // The order of files should be from most Important to lower
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
