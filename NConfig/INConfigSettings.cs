using System.IO;

namespace NConfig
{
    internal interface INConfigSettings
    {
        string HostAlias { get; }
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
