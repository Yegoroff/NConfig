using System.Configuration;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Collections.Specialized;

namespace NConfig
{
    /// <summary>
    ///   Provides access to configuration files for client applications.
    /// </summary>
    public interface INConfiguration
    {

        /// <summary>
        ///     Gets the System.Configuration.ConnectionStringsSection data for the
        ///     current configuration file set.
        /// </summary>
        /// <value>
        ///     A System.Configuration.ConnectionStringSettingsCollection object
        ///     that contains the contents of the System.Configuration.ConnectionStringsSection
        ///     object for the current configuration file set.
        /// </value>
        ConnectionStringSettingsCollection ConnectionStrings { get; }

        /// <summary>
        ///     Gets the System.Configuration.AppSettingsSection data for the 
        ///     current configuration file set.
        /// </summary>
        /// <value>
        ///     A System.Collections.Specialized.NameValueCollection object that
        ///     contains the contents of the System.Configuration.AppSettingsSection object
        ///     for the current configuration file set.
        /// </value>
        NameValueCollection AppSettings { get; }

        /// <summary>
        ///     Gets the list of configuration file names assigned to current configuration.
        /// </summary>
        /// <value>The configuration file names.</value>
        IList<string> FileNames { get; }

        /// <summary>
        ///     Retrieves a specified configuration section for the 
        ///     current configuration file set.
        /// </summary>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>
        ///     The specified System.Configuration.ConfigurationSection object, 
        ///     or null if the section does not exist.
        /// </returns>
        ConfigurationSection GetSection(string sectionName);

        /// <summary>
        ///     Gets the specified <see cref="System.Configuration.ConfigurationSectionGroup"/> object.
        /// </summary>
        /// <param name="groupName"> The path name of the <see cref="System.Configuration.ConfigurationSectionGroup"/> to return.</param>
        /// <returns>The <see cref="System.Configuration.ConfigurationSectionGroup"/> specified.</returns>
        ConfigurationSectionGroup GetSectionGroup(string groupName);
    }



    /// <summary>
    /// Add default and typed extensions to IConfiguration interface.
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
