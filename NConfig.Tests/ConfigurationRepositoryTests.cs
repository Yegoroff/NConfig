using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NConfig.Tests
{
    [TestFixture]
    public class ConfigurationRepositoryTests
    {

        [Test]
        public void Should_read_Configuration_from_specified_file_path()
        {
            ConfigurationRepository reposiotryUnderTest = new ConfigurationRepository();
            string configurationFile = "Configs\\NConfigTest.config";

            Configuration readConfiguration =  reposiotryUnderTest.GetFileConfiguration(configurationFile);

            Assert.That(readConfiguration, Is.Not.Null);
        }


        [Test]
        public void Should_cache_Configuration_by_file_paths()
        {
            ConfigurationRepository reposiotryUnderTest = new ConfigurationRepository();
            string configurationFile = "Configs\\NConfigTest.config";

            Configuration readConfiguration = reposiotryUnderTest.GetFileConfiguration(configurationFile);
            Configuration cachedConfiguration = reposiotryUnderTest.GetFileConfiguration(configurationFile);

            Assert.That(cachedConfiguration, Is.SameAs(readConfiguration));
        }


        [Test]
        public void Should_convert_passed_relative_paths_to_absolute_correctly()
        {
            ConfigurationRepositoryAbsoltePathTester reposiotryUnderTest = new ConfigurationRepositoryAbsoltePathTester();
            string configurationFile = "Configs\\NConfigTest.config";

            string validPath = AppDomain.CurrentDomain.BaseDirectory;
            validPath = Path.Combine(Path.GetDirectoryName(validPath), configurationFile);

            string absolutePath = reposiotryUnderTest.TestToAbsolutePath(configurationFile);

            Assert.That(absolutePath, Is.EqualTo(validPath));
        }


        [Test]
        public void Should_not_modify_passed_absolute_path()
        {
            ConfigurationRepositoryAbsoltePathTester reposiotryUnderTest = new ConfigurationRepositoryAbsoltePathTester();
            string configurationFile = "D:\\Configs\\NConfigTest.config";

            string absolutePath = reposiotryUnderTest.TestToAbsolutePath(configurationFile);

            Assert.That(absolutePath, Is.EqualTo(configurationFile));
        }

    }

    internal class ConfigurationRepositoryAbsoltePathTester : ConfigurationRepository
    {
        public string TestToAbsolutePath(string path)
        {
            return this.ToAbsolutePath(path);
        }
    }

}

