using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System;

namespace NConfig
{
    internal class ConfigurationRepository : IConfigurationRepository
    {
        private readonly Dictionary<string, Configuration> configCache = new Dictionary<string, Configuration>();
        private static readonly string rootPath = GetRootPath();

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
                    var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
                    result = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    configCache.Add(filePath, result);
                }
                return result;
            }
        }


        public string DumpDiagnostics(IList<string> preconfiguredFilenames)
        {
            string result = "";
            foreach (var fileName in preconfiguredFilenames)
            {
                var filePath = ToAbsolutePath(fileName);
                string fileState = "missing";
                if (File.Exists(filePath))
                {
                    fileState =  configCache.ContainsKey(filePath) ? "applied" : "exists";                    
                }

                result += fileState + " file: '" + fileName + "' location: '" + filePath + "' \r\n";
            }
            return result;
        }


        protected virtual string ToAbsolutePath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Combine(rootPath, path);

            return Path.GetFullPath(path);
        }

        private static string GetRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
