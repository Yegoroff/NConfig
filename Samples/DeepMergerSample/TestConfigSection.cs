using System.Configuration;
using NConfig;

namespace DeepMergerSample
{
    public class TestConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("MainProperty")]
        public FirstLayerElement TestValue {
            get {
                return (FirstLayerElement)this["MainProperty"];
            }
           
        }
    }

    public class FirstLayerElement : ConfigurationElement
    {
        [ConfigurationProperty("Attribute")]
        public string Attribute {
            get {
                return (string)this["Attribute"];
            }
        }

        [ConfigurationProperty("Collection")]
        public TestCollection Collection { get { return (TestCollection) this["Collection"]; } }
    }

    [ConfigurationCollection(typeof(SecondLayerElement), AddItemName = "addOrSo",
            CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class TestCollection : MergeableConfigurationCollection<SecondLayerElement> { }


    public class SecondLayerElement : ConfigurationElement
    {
        [ConfigurationProperty("Key", IsKey = true)]
        public string Key
        {
            get { return (string)this["Key"]; }
        }

        
        [ConfigurationProperty("Attribute")]
        public string Attribute
        {
            get { return (string) this["Attribute"]; }
        }
        }
}
