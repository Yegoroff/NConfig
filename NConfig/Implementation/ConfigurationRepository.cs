using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace NConfig
{
    internal class ConfigurationRepository : IConfigurationRepository
    {
        private static readonly Dictionary<string, Configuration> configCache = new Dictionary<string, Configuration>();


        public Configuration GetFileConfiguration(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            lock (configCache)
            {
                string filePath = Path.GetFullPath(fileName);
                Configuration result;
                if (!configCache.TryGetValue(filePath, out result))
                {
                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = filePath };
                    result = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    if (result != null)
                        configCache.Add(filePath, result);
                }
                return result;
            }
        }
    }
}
