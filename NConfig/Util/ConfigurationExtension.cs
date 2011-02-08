using System.Configuration;

namespace NConfig
{
    /// <summary>
    /// Provides extensions to <see cref="System.Configuration.Configuration"/> type.
    /// </summary>
    public static class ConfigurationExtension
    {
        /// <summary>
        /// Retrieves configuration section with a name that corresponds to <typeparamref name="T"/> short type name.
        /// </summary>
        /// <typeparam name="T">Configuration section Type that inherits <see cref="System.Configuration.ConfigurationSection"/>.</typeparam>
        /// <param name="config"></param>
        /// <returns>
        /// The specified ConfigurationSection object of type <typeparamref name="T"/>,
        /// or null if the section does not exist.
        /// </returns>
        public static T GetSection<T>(this Configuration config) where T : ConfigurationSection
        {
            return config.GetSection<T>(typeof(T).Name);
        }


        /// <summary>
        /// Retrieves a specified configuration section of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Configuration section Type that inherits <see cref="System.Configuration.ConfigurationSection"/>.</typeparam>
        /// <param name="config"></param>
        /// <param name="sectionName">The configuration section path and name.</param>
        /// <returns>
        /// The specified ConfigurationSection object of type <typeparamref name="T"/>,
        /// or null if the section does not exist.
        /// </returns>
        public static T GetSection<T>(this Configuration config, string sectionName) where T : ConfigurationSection
        {
            return config.GetSection(sectionName) as T;
        }
    }
}
