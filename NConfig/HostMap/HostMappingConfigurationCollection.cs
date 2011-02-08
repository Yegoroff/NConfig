using System;
using System.Configuration;
using System.Linq;

namespace NConfig
{
    /// <summary>
    /// Host names to Aliases mapping collection.
    /// </summary>
    [ConfigurationCollection(typeof(HostMappingConfigurationCollection))]
    public class HostMappingConfigurationCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();


        /// <summary>
        /// Initializes a new instance of the <see cref="HostMappingConfigurationCollection"/> class.
        /// </summary>
        public HostMappingConfigurationCollection() : base(StringComparer.OrdinalIgnoreCase) { }


        /// <summary>
        /// Gets all registered host names.
        /// </summary>
        /// <value>The array of registered host names.</value>
        public string[] AllHosts
        {
            get
            {
                return base.BaseGetAllKeys().Cast<string>().ToArray();
            }
        }


        /// <summary>
        /// Gets the <see cref="NConfig.HostMappingConfigurationElement"/> with the specified host name.
        /// </summary>
        public new HostMappingConfigurationElement this[string host]
        {
            get
            {
                return (HostMappingConfigurationElement)base.BaseGet(host);
            }
        }


        /// <summary>
        /// Determines whether host mapping contains specified host.
        /// </summary>
        /// <param name="host">The host name to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified host regitered; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsHost(string host)
        {
            return base.BaseGet(host) != null;            
        }

        /// <summary>
        /// Adds the specified host mapping.
        /// </summary>
        /// <param name="hostMapping">The host mapping.</param>
        public void Add(HostMappingConfigurationElement hostMapping)
        {
            hostMapping.Initialize();

            HostMappingConfigurationElement element = (HostMappingConfigurationElement)base.BaseGet(hostMapping.Host);
            if (element == null)
            {
                BaseAdd(hostMapping);
            }
            else
            {
                element.Alias = hostMapping.Alias;
                int index = BaseIndexOf(element);
                BaseRemoveAt(index);
                BaseAdd(index, element);
            }
        }

        /// <summary>
        /// Adds the specified host name and alias pair.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="alias">The alias that corresponds to this host.</param>
        public void Add(string host, string alias)
        {
            HostMappingConfigurationElement keyValue = new HostMappingConfigurationElement(host, alias);
            Add(keyValue);
        }

        /// <summary>
        ///  Removes all registrations from the mapping.
        /// </summary>
        public void Clear()
        {
            base.BaseClear();
        }

        /// <summary>
        /// Removes the specified host registration from the mapping.
        /// </summary>
        /// <param name="host">The host name.</param>
        public void Remove(string host)
        {
            base.BaseRemove(host);
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
        /// Gets a value indicating whether an attempt to add a duplicate <see cref="T:System.Configuration.ConfigurationElement"/> to the <see cref="T:System.Configuration.ConfigurationElementCollection"/> will cause an exception to be thrown.
        /// </summary>
        /// <returns>true if an attempt to add a duplicate <see cref="T:System.Configuration.ConfigurationElement"/> to this <see cref="T:System.Configuration.ConfigurationElementCollection"/> will cause an exception to be thrown; otherwise, false. </returns>
        protected override bool ThrowOnDuplicate
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new HostMappingConfigurationElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HostMappingConfigurationElement)element).Host;
        }
    
    }

}
