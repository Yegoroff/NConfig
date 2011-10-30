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
    public class ConfigurationRepositoryWebTests
    {

        [Test]
        public void Should_not_modify_passed_absolute_path()
        {
            ConfigurationRepositoryWebAbsoltePathTester reposiotryUnderTest = new ConfigurationRepositoryWebAbsoltePathTester();
            string configurationFile = "D:\\Configs\\NConfigTest.config";

            string absolutePath = reposiotryUnderTest.TestToAbsolutePath(configurationFile);

            Assert.That(absolutePath, Is.EqualTo(configurationFile));
        }

    }

    internal class ConfigurationRepositoryWebAbsoltePathTester : ConfigurationRepositoryWeb
    {
        public string TestToAbsolutePath(string path)
        {
            return this.ToAbsolutePath(path);
        }
    }

}

