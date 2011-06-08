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
        ///     Retrieves an untyped specified configuration section for the 
        ///     current configuration file set.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>
        ///     The specified configuration section object, 
        ///     or null if the section does not exist.
        /// </returns>
        object GetSectionUntyped(string sectionName);

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
        /// <summary>
        /// Retrieves configuration section with a name that corresponds to <typeparamref name="T"/> short type name.
        /// </summary>
        /// <typeparam name="T">Configuration section Type that inherits <see cref="System.Configuration.ConfigurationSection"/>.</typeparam>
        /// <returns>
        /// The specified ConfigurationSection object of type <typeparamref name="T"/>,
        /// or null if the section does not exist.
        /// </returns>
        public static T GetSection<T>(this INConfiguration configuration) where T : ConfigurationSection
        {
            return configuration.GetSection<T>(typeof(T).Name);
        }

        /// <summary>
        /// Retrieves a specified configuration section of type <typeparamref name="T"/> for the
        /// current configuration file set.
        /// </summary>
        /// <typeparam name="T">Configuration section Type that inherits <see cref="System.Configuration.ConfigurationSection"/>.</typeparam>
        /// <param name="configuration"></param>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>
        /// The specified ConfigurationSection object of type <typeparamref name="T"/>,
        /// or null if the section does not exist.
        /// </returns>
        public static T GetSection<T>(this INConfiguration configuration, string sectionName) where T : ConfigurationSection
        {
            return configuration.GetSection(sectionName) as T;
        }

        /// <summary>
        /// Gets the specified Configuration Section Group object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Configuration section group Type that inherits <see cref="System.Configuration.ConfigurationSectionGroup"/>.</typeparam>
        /// <param name="configuration"></param>
        /// <param name="groupName">The path name of the <see cref="System.Configuration.ConfigurationSectionGroup"/> to return.</param>
        /// <returns>The section group specified.</returns>
        public static T GetGetSectionGroup<T>(this INConfiguration configuration, string groupName) where T : ConfigurationSectionGroup
        {
            return configuration.GetSectionGroup(groupName) as T;
        }

        /// <summary>
        /// Sets this configuration as the NConfigurator default configuration.
        /// </summary>
        public static INConfiguration SetAsDefault(this INConfiguration config)
        {
            NConfigurator.Default = config;
            return config;
        }

        /// <summary>
        /// Sets this configuration as the NConfigurator default and System wide default configuration.
        /// Thus you can use <see cref="System.Configuration.ConfigurationManager"/> to access this configuration.
        /// </summary>
        public static INConfiguration SetAsSystemDefault(this INConfiguration config)
        {
            NConfigurator.Default = config;
            
            if (NConfigurator.Settings.IsWeb)
                NSystemConfigurator.SubstituteWebConfigSystem(NConfigurator.ConfigurationFactory, config.FileNames);
            else
                NSystemConfigurator.SubstituteClientConfigSystem(NConfigurator.ConfigurationFactory, config.FileNames);
            return config;
        }

    }
}
