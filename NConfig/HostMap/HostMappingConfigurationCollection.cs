using System;
using System.Configuration;
using System.Linq;

namespace NConfig
{
    [ConfigurationCollection(typeof(HostMappingConfigurationCollection))]
    public class HostMappingConfigurationCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        

        public HostMappingConfigurationCollection() : base(StringComparer.OrdinalIgnoreCase) { }


        public string[] AllHosts
        {
            get
            {
                return base.BaseGetAllKeys().Cast<string>().ToArray();
            }
        }

        public new HostMappingConfigurationElement this[string key]
        {
            get
            {
                return (HostMappingConfigurationElement)base.BaseGet(key);
            }
        }


        public bool ContainsHost(string host)
        {
            return base.BaseGet(host) != null;            
        }

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

        public void Add(string host, string alias)
        {
            HostMappingConfigurationElement keyValue = new HostMappingConfigurationElement(host, alias);
            Add(keyValue);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void Remove(string key)
        {
            base.BaseRemove(key);
        }


        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        protected override bool ThrowOnDuplicate
        {
            get
            {
                return false;
            }
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new HostMappingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HostMappingConfigurationElement)element).Host;
        }
    
    }

}
