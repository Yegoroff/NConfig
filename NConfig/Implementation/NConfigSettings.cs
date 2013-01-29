using System;
using System.Configuration;
using System.IO;

namespace NConfig
{
    internal sealed class NConfigSettings : INConfigSettings
    {
        private const string HostMapFile = "HostMap.config";

        private readonly IConfigurationRepository repository;
        private readonly bool isWeb;
        private string hostAlias;

        public NConfigSettings(IConfigurationRepository repository)
        {
            this.repository = repository;
            isWeb = DetectIsWeb();
            hostAlias = DetectHostAlias();
        }


        /// <summary>
        /// Gets or sets the alias assigned for current Host.
        /// This alias used to find out Host specific configurations.
        /// </summary>
        /// <value>The host's alias.</value>
        public string HostAlias
        {
            get
            {
                return hostAlias;
            }
            set
            {
                hostAlias = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether current application is web based.
        /// </summary>
        /// <value><c>true</c> if current application is web bases; otherwise, <c>false</c>.</value>
        public bool IsWeb
        {
            get
            {
                return isWeb;
            }
        }


        /// <summary>
        /// Detects the alias for the current host.
        /// First it reads HostMap.Config file then searches inside App.Config, if not successful
        /// returns current host name.
        /// </summary>
        private string DetectHostAlias()
        {
            // Try to read from HostMap.config file, then try to read from AppConfig/WebConfig
            string hostName = Environment.MachineName;
            HostMapSection hostMapSection = null;

            var hostMapFilename = IsWeb ? @"~/" + HostMapFile : HostMapFile;
            Configuration hostConfig = repository.GetFileConfiguration(hostMapFilename);
            if (hostConfig != null)
                hostMapSection = hostConfig.GetSection<HostMapSection>("hostMap");

            if (hostMapSection == null)
                hostMapSection = ConfigurationManager.GetSection("hostMap") as HostMapSection;

            if (hostMapSection != null)
            {
                if (hostMapSection.Mappings.ContainsHost(hostName))
                    return hostMapSection.Mappings[hostName].Alias;

                // Wildcard alias mapping.
                if (hostMapSection.Mappings.ContainsHost("*"))
                    return hostMapSection.Mappings["*"].Alias;
            }
            return hostName;
        }

        /// <summary>
        /// Detects if the current application is web based.
        /// Detection method is not natural (HostingEnvironment.IsHosted) but allows no to upload System.Web assembly.
        /// </summary>
        public static bool DetectIsWeb()
        {
            string configFile = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            return configFile != null && configFile.Equals("web.config", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
