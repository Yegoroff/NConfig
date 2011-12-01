using System.Configuration;

namespace NConfig.Tests
{
    public class MergeSection : ConfigurationSection
    {

        [ConfigurationProperty("StringValue", DefaultValue = "DEFAULT")]
        public string StringValue
        {
            get { return (string)this["StringValue"]; }
            set { this["StringValue"] = value; }
        }

        [ConfigurationProperty("IntValue", DefaultValue = "1234")]
        public int IntValue
        {
            get { return (int)this["IntValue"]; }
            set { this["IntValue"] = value; }
        }

        [ConfigurationProperty("BoolValue", DefaultValue = "true")]
        public bool BoolValue
        {
            get { return (bool)this["BoolValue"]; }
            set { this["BoolValue"] = value; }
        }

    }
}
