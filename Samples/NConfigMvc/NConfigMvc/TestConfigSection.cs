using System.Configuration;

namespace NConfigMvc
{
    public class TestConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("TestValue")]
        public string TestValue
        {
            get
            {
                return (string)this["TestValue"];
            }
            set
            {
                this["TestValue"] = value;
            }
        }
    }
}