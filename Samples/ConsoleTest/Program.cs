using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfig;
using System.Configuration;
using System.Configuration.Internal;
using System.Reflection;
using System.Globalization;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");


            NConfigurator.UsingFile("Config\\HostMap.config").PromoteToDefault();


            TestConfig tc =  NConfigurator.Default.GetSection<TestConfig>();
            Console.WriteLine(tc.TestValue);

            object sect = ConfigurationManager.GetSection("TestConfig");

            NConfigurator.UsingFile("Config\\HostMap.config").GetSection<TestConfig>("testConfig");


            Console.WriteLine(NConfigurator.UsingFile("Config\\HostMap.config").ConnectionStrings.Count);



            TestConfig c = NConfigurator.UsingFile("Config\\HostMap.config").GetSection<TestConfig>();
            Console.WriteLine(c.TestValue);

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }


}
