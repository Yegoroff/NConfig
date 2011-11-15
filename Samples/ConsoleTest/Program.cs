using System;
using NConfig;
using System.Configuration;

namespace ConsoleTest
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Simple NConfig Sample. \r\n");

            // Setup NConfigurator to use Custom.config file from Config subfolder.
            NConfigurator.UsingFile(@"Config\Custom.config").SetAsSystemDefault();


            var testSection = NConfigurator.Default.GetSection<TestConfigSection>();

            var configManagerTestSection = ConfigurationManager.GetSection("TestConfigSection") as TestConfigSection;

            var namedTestSection = NConfigurator.UsingFile(@"Config\Custom.config").GetSection<TestConfigSection>("NamedSection");


            Console.WriteLine("NConfig Default : {0} \r\n".F(testSection.TestValue));
            Console.WriteLine("ConfigurationManager : {0} \r\n".F(configManagerTestSection.TestValue));
            Console.WriteLine("NConfig named section : {0} \r\n".F(namedTestSection.TestValue));

            Console.WriteLine("");
            Console.WriteLine("MERGED APP SETTINGS : ");
            foreach(var key in ConfigurationManager.AppSettings.AllKeys)
            {
                Console.WriteLine(key + " : " +  ConfigurationManager.AppSettings[key]);
            }

            Console.WriteLine("");
            Console.WriteLine("MERGED CONNECTION STRINGS : ");
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                Console.WriteLine(connectionString.Name + " : " + connectionString.ConnectionString);
            }

            Console.ReadKey();
        }
    }

    static class StringExtensions
    {
        public static string F(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }

}
