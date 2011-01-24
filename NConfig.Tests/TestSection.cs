using System.Configuration;

namespace NConfig.Tests
{
    public class TestSection : ConfigurationSection
    {

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)this["value"]; }
        }

    }
}
