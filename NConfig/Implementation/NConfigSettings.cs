using System;
using System.Configuration;
using System.Linq;

namespace NConfig
{
    internal sealed class NConfigSettings : INConfigSettings
    {
        private const string HostMapFile = "HostMap.config";

        private readonly IConfigurationRepository repository;
        private readonly string hostAlias;


        public NConfigSettings(IConfigurationRepository repository)
        {
            this.repository = repository;
            hostAlias = DetectHostAlias();
        }


        public string HostAlias
        {
            get
            {
                return hostAlias;
            }
        }


        private string DetectHostAlias()
        {
            // Try to read from HostMap.config file, then try to read from AppConfig/WebConfig
            string hostName = Environment.MachineName;
            HostMapSection hostMapSection = null;

            Configuration hostConfig = repository.GetFileConfiguration(HostMapFile);
            if (hostConfig != null)
                hostMapSection = hostConfig.GetSection<HostMapSection>("hostMap");

            if (hostMapSection == null)
                hostMapSection = ConfigurationManager.GetSection("hostMap") as HostMapSection;

            if (hostMapSection != null)
            {
                if (hostMapSection.Mappings.ContainsHost(hostName))
                    return hostMapSection.Mappings[hostName].Alias;
            }
            return hostName;
        }
    }
}
