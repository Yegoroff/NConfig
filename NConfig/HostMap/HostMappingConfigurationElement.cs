using System.Configuration;

namespace NConfig
{

    public class HostMappingConfigurationElement : ConfigurationElement
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty propHost = new ConfigurationProperty("host", typeof(string), string.Empty, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty propAlias = new ConfigurationProperty("alias", typeof(string), string.Empty, ConfigurationPropertyOptions.None);

        private string initHost;
        private string initAlias;
        private bool needsInit;


        static HostMappingConfigurationElement()
        {
            properties.Add(propHost);
            properties.Add(propAlias);
        }

        public HostMappingConfigurationElement() { }

        public HostMappingConfigurationElement(string host, string alias)
        {
            needsInit = true;
            initHost = host;
            initAlias = alias;
        }


        [ConfigurationProperty("host", Options = ConfigurationPropertyOptions.IsKey, DefaultValue = "")]
        public string Host
        {
            get
            {
                return (string)base[propHost];
            }
        }

        [ConfigurationProperty("alias", DefaultValue = "")]
        public string Alias
        {
            get
            {
                return (string)base[propAlias];
            }
            set
            {
                base[propAlias] = value;
            }
        }


        internal void Initialize()
        {
            Init();
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        protected override void Init()
        {
            base.Init();
            if (needsInit)
            {
                needsInit = false;
                base[propHost] = initHost;
                Alias = initAlias;
            }
        }
    }
}
