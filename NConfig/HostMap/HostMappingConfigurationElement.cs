using System.Configuration;

namespace NConfig
{

    /// <summary>
    /// Represents Host to HostAlias mapping.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HostMappingConfigurationElement"/> class.
        /// </summary>
        public HostMappingConfigurationElement() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostMappingConfigurationElement"/> class.
        /// </summary>
        /// <param name="host">The Host name.</param>
        /// <param name="alias">The Alias to map Host to.</param>
        public HostMappingConfigurationElement(string host, string alias)
        {
            needsInit = true;
            initHost = host;
            initAlias = alias;
        }


        /// <summary>
        /// Gets the Host Name.
        /// </summary>
        /// <value>The Host Name.</value>
        [ConfigurationProperty("host", Options = ConfigurationPropertyOptions.IsKey, DefaultValue = "")]
        public string Host
        {
            get
            {
                return (string)base[propHost];
            }
        }

        /// <summary>
        /// Gets or sets the Alias.
        /// </summary>
        /// <value>The Alias.</value>
        /// <remarks>This property is writable to allow mapping merging acros different configs.</remarks>
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


        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> of properties for the element.</returns>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        /// <summary>
        /// Sets the <see cref="T:System.Configuration.ConfigurationElement"/> object to its initial state.
        /// </summary>
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
