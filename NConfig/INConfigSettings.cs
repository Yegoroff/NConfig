using System.IO;

namespace NConfig
{
    /// <summary>
    /// Represents the current configuration of NConfigurator.
    /// Use it to check and set host alias.
    /// </summary>
    public interface INConfigSettings
    {
        /// <summary>
        /// Gets or Sets the host alias used to read configuration file.
        /// This settings populated automatically based on HostMap configuration and current computer name.
        /// You can set it manually if you need to handle some complex deployment scenarios.
        /// </summary>
        string HostAlias { get; set; }

        /// <summary>
        /// Reflects whether current environment is web.
        /// </summary>
        bool IsWeb { get; }
    }


    internal static class INConfigSettingsExtension
    {
        public static string GetAliasedFileName(this INConfigSettings settings, string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(fileName),
                           settings.HostAlias + "." + Path.GetFileName(fileName));
        }
    }
}
