using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ConsoleTest
{
    public class TestConfig : ConfigurationSection
    {

        

        [ConfigurationProperty("testValue")]
        public string TestValue
        {
            get
            {
                return (string)this["testValue"];
            }
            set
            {
                this["testValue"] = value;
            }
        }


    }
}
