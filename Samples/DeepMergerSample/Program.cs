using System;
using NConfig;
using System.Configuration;

namespace DeepMergerSample
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("DeepMerger NConfig Sample. \r\n");

            // Registering the correct merger for my section
            NConfigurator.RegisterSectionMerger(new DeepMerger<TestConfigSection>());

            // Setup NConfigurator to use Custom.config file from Config subfolder.
            NConfigurator.UsingFile(@"Config\Custom.config").SetAsSystemDefault();

            var testSection = NConfigurator.Default.GetSection<TestConfigSection>();
            

            Console.WriteLine("Property Attribute: {0} \r\n".F(testSection.TestValue.Attribute));

            Console.WriteLine("");
            Console.WriteLine("MERGED COLLECTION: ");
            foreach(var key in testSection.TestValue.Collection)
            {
                Console.WriteLine(key.Key + " : " + key.Attribute);
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
