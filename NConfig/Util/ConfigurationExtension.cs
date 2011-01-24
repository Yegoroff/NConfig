using System.Configuration;

namespace NConfig
{
    public static class ConfigurationExtension
    {

        public static T GetSection<T>(this Configuration config) where T : ConfigurationSection
        {
            return config.GetSection<T>(typeof(T).Name);
        }

        public static T GetSection<T>(this Configuration config, string sectionName) where T : ConfigurationSection
        {
            return config.GetSection(sectionName) as T;
        }
    }
}
