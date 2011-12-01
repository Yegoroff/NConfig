using System.Configuration;

namespace NConfig.Tests
{
    public class TestSection : ConfigurationSection
    {

        [ConfigurationProperty("value", DefaultValue = "DEFAULT")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

    }
}
