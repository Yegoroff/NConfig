using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
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
        // This is because IConfigurationSectionHandler is deprecated, but still a lot of 3rd party libraries use it. (log4net, nlog etc.)
        // These sections returned as DefaultSection from Configuration.GetSection so we need to handle such sections by our self.
        private object HandleIConfigurationSectionHandlerSection(DefaultSection section)
        {
            string sectionTypeName = section.SectionInformation.Type;
            var sectionType = Type.GetType(sectionTypeName);

            if (typeof (IConfigurationSectionHandler).IsAssignableFrom(sectionType))
            {
                var ctor = sectionType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (ctor == null)
                    throw new InvalidOperationException("Can't instantiate section handler without default ctor.");

                // Create IConfigurationSectionHandler for this section.
                var handler = Activator.CreateInstance(sectionType, true) as IConfigurationSectionHandler;
                if (handler == null)
                    return null;

                var rawXml = section.SectionInformation.GetRawXml();
                if (string.IsNullOrEmpty(rawXml))
                    return null;

                var doc = new XmlDocument();
                doc.LoadXml(rawXml);

                // Invoke IConfigurationSectionHandler.Create method with read rawXlm as parameter.
                // We pass DocumentElement because some 3rd party libs expects XmlElement passed, not XmlNode as declared in Create signature.
                return handler.Create(null, null, doc.DocumentElement);
            }

            return null;
        }

        private bool IsSectionPresentInConfigFile(ConfigurationSection section)
        {
            // Default session always has ElementInformation.IsPresent == false. So we just check content availability.
            if (section is DefaultSection)
                return !string.IsNullOrEmpty(section.SectionInformation.GetRawXml());

            return section.ElementInformation.IsPresent;
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
                // This property couldn't be cached, because of possible changes to app settings
                // and nested configuration files on web
                return GetConnectionStrings();
            }
        }

        public NameValueCollection AppSettings
        {
            get
            {
                // This property couldn't be cached, because of possible changes to app settings
                // and nested configuration files on webs
                return GetAppSettings();
            }
        }


        public ConfigurationSection GetSection(string sectionName)
        {
            var sections = new List<ConfigurationSection>();
            
            // Read section from custom configuration files.
            ConfigurationSection emptySection = null;
            ConfigurationSection section;

            foreach (string fileName in FileNames) // The order of files should be from most Important to lower
            {
                section = GetFileSection(fileName, sectionName);
                
                // Filter out non-present in file sections (IsPresent == false).
                if (section != null)
                {
                    if ( IsSectionPresentInConfigFile(section))
                        sections.Add(section);
                    
                    // Do not add non present DefaultSections, they will be bank - so no handler processing occurred.
                    // Also this could prevent from returning Default system section. (as with section "system.web/browserCaps" under web)
                    else if (!(section is DefaultSection) && section.SectionInformation.IsDeclared)
                        emptySection = section; // declared but not present section - should be used if nothing else found.
                }
            }

            // Read Section form Configuration Manager.
            section = GetDefaultSection(sectionName);
            if (section != null) // Add not presented sections too, because of non required sections should be returned.
                sections.Add(section);


            if (emptySection != null)
                sections.Add(emptySection);

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
