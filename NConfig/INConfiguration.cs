using System.Configuration;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Collections.Specialized;

namespace NConfig
{
    public interface INConfiguration
    {
        ConnectionStringSettingsCollection ConnectionStrings { get; }

        NameValueCollection AppSettings { get; }

        IList<string> FileNames { get; }

        ConfigurationSection GetSection(string sectionName);

        ConfigurationSectionGroup GetSectionGroup(string groupName);
    }



    /// <summary>
    /// Add typed extensions to IConfiguration interface.
    /// </summary>
    public static class INConfigurationExtensions
    {

        public static T GetSection<T>(this INConfiguration configuration) where T : ConfigurationSection
        {
            return configuration.GetSection<T>(typeof(T).Name);
        }

        public static T GetSection<T>(this INConfiguration configuration, string sectionName) where T : ConfigurationSection
        {
            return configuration.GetSection(sectionName) as T;
        }

        public static T GetGetSectionGroup<T>(this INConfiguration configuration, string groupName) where T : ConfigurationSectionGroup
        {
            return configuration.GetSectionGroup(groupName) as T;
        }

        public static INConfiguration PromoteToDefault(this INConfiguration config)
        {
            NConfigurator.Default = config;
            return config;
        }

        public static INConfiguration PromoteToSystemDefault(this INConfiguration config)
        {
            NConfigurator.Default = config;
            NSystemDefaultConfig.SubstituteInternalConfigSystem(new NSystemDefaultConfig(NConfigurator.Repository, config.FileNames));
            return config;
        }

    }
}
