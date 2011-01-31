using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace NConfig
{
    internal class ConfigurationRepository : IConfigurationRepository
    {
        private readonly Dictionary<string, Configuration> configCache = new Dictionary<string, Configuration>();


        public Configuration GetFileConfiguration(string fileName)
        {
            string filePath = ToAbsolutePath(fileName);

            if (!File.Exists(filePath))
                return null;

            lock (configCache)
            {
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

        protected virtual string ToAbsolutePath(string path)
        {
            return Path.GetFullPath(path);
        }

    }
}
