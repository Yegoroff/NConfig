using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NConfig;
using System.Configuration;

namespace NConfig.Tests
{
    [TestFixture]
    public class NConfiguratorTests
    {

        [Test]
        public void ReadConectStringsFromNotExistingFile()
        {
            var connStrings = NConfigurator.FromFile("NotExisting.config").ConnectionStrings;

            Assert.That(connStrings, Is.EqualTo(ConfigurationManager.ConnectionStrings));
        }

        [Test]
        public void ReadAppSettingsFromNotExistingFile()
        {
            var settings = NConfigurator.FromFile("NotExisting.config").AppSettings; 

            Assert.That(settings, Is.EqualTo(ConfigurationManager.AppSettings));
        }

        [Test]
        public void MergeAppSettingsWithFile()
        {
            var settings = NConfigurator.FromFile("Configs//NConfigTest.config").AppSettings;

            Assert.That(settings.Count, Is.EqualTo(3));
            Assert.That(settings["Test"], Is.EqualTo("NConfigTest.Value"));
        }

        [Test]
        public void MergeConectStringsWithFile()
        {
            var connStrings = NConfigurator.FromFile("Configs//NConfigTest.config").ConnectionStrings;

            Assert.That(connStrings.Count, Is.EqualTo(4));
            Assert.That(connStrings["TestConnectString"].ConnectionString, Is.EqualTo("NConfigTest.ConnectString"));
        }


        [Test]
        public void MultifileConfiguration()
        {
            var testSection = NConfigurator.FromFiles("Configs//Aliased.config", "Configs//NConfigTest.config").GetSection<TestSection>();
            var conString = NConfigurator.FromFiles("Configs//Aliased.config", "Configs//NConfigTest.config").ConnectionStrings["TestConnectString"].ConnectionString;

            Assert.That(testSection.Value, Is.EqualTo("Tests.Aliased.Value"));
            Assert.That(conString, Is.EqualTo("Aliased.ConnectString"));
        }


        [Test]
        public void DefaultConfigCorrespondsToConfigurationManager()
        {
            Assert.That(NConfigurator.Default.ConnectionStrings, Is.EqualTo(ConfigurationManager.ConnectionStrings));
            Assert.That(NConfigurator.Default.AppSettings, Is.EqualTo(ConfigurationManager.AppSettings));
        }


        [Test]
        public void DefaultConfigPromotion()
        {
            try
            {
                NConfigurator.FromFile("Configs//NConfigTest.config").PromoteToDefault();

                Assert.That(NConfigurator.Default.FileNames, Is.EqualTo(NConfigurator.FromFile("Configs//NConfigTest.config").FileNames));
                Assert.That(NConfigurator.Default.ConnectionStrings, Is.EqualTo(NConfigurator.FromFile("Configs//NConfigTest.config").ConnectionStrings));
                Assert.That(NConfigurator.Default.AppSettings, Is.EqualTo(NConfigurator.FromFile("Configs//NConfigTest.config").AppSettings));
            }
            finally
            {
                NConfigurator.FromFiles().PromoteToDefault(); // Restore default Default
            }

        }


        [Test]
        public void SystemDefaultConfigPromotion()
        {
            try
            {
                var connectStrings = NConfigurator.FromFile("Configs//NConfigTest.config").ConnectionStrings;
                var appSettings = NConfigurator.FromFile("Configs//NConfigTest.config").AppSettings;
                NConfigurator.FromFile("Configs//NConfigTest.config").PromoteToSystemDefault();

                Assert.That(ConfigurationManager.ConnectionStrings, Is.EqualTo(connectStrings));
                Assert.That(ConfigurationManager.AppSettings, Is.EqualTo(appSettings));
            }
            finally
            {
                NConfigurator.RestoreSystemDefaults();
            }
        }


    }
}
