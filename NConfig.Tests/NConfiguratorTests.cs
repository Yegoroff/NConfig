using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using System.Diagnostics;
using System.IO;

namespace NConfig.Tests
{
    [TestFixture]
    public class NConfiguratorTests
    {

        [Test]
        public void Should_provide_default_ConectStrings_for_non_existing_file()
        {
            var connStrings = NConfigurator.UsingFile("NotExisting.config").ConnectionStrings;

            Assert.That(connStrings, Is.EqualTo(ConfigurationManager.ConnectionStrings));
        }

        [Test]
        public void Should_provide_default_AppSettings_for_non_existing_file()
        {
            var settings = NConfigurator.UsingFile("NotExisting.config").AppSettings; 

            Assert.That(settings, Is.EqualTo(ConfigurationManager.AppSettings));
        }

        [Test]
        public void Should_merge_default_AppSettings_with_defined_in_file()
        {
            var settings = NConfigurator.UsingFile("Configs\\NConfigTest.config").AppSettings;

            Assert.That(settings.Count, Is.EqualTo(3));
            Assert.That(settings["Test"], Is.EqualTo("NConfigTest"));
        }

        [Test]
        public void Should_merge_default_ConectStrings_with_defined_in_file()
        {
            var connStrings = NConfigurator.UsingFile("Configs\\NConfigTest.config").ConnectionStrings;

            Assert.That(connStrings.Count, Is.EqualTo(4));
            Assert.That(connStrings["TestConnectString"].ConnectionString, Is.EqualTo("NConfigTest"));
        }

        [Test]
        public void Should_override_sections_by_the_most_recent_config_file()
        {
            var testSection = NConfigurator.UsingFiles("Configs\\Aliased.config", "Configs\\NConfigTest.config").GetSection<TestSection>();

            Assert.That(testSection.Value, Is.EqualTo("Tests.Aliased"));
        }

        [Test]
        public void Should_override_ConnectString_by_the_most_recent_config_file()
        {
            var conString = NConfigurator.UsingFiles("Configs\\Aliased.config", "Configs\\NConfigTest.config").ConnectionStrings["TestConnectString"].ConnectionString;

            Assert.That(conString, Is.EqualTo("Aliased"));
        }

        [Test]
        public void Should_corresponds_to_ConfigurationManager_by_default()
        {
            Assert.That(NConfigurator.Default.ConnectionStrings, Is.EqualTo(ConfigurationManager.ConnectionStrings));
            Assert.That(NConfigurator.Default.AppSettings, Is.EqualTo(ConfigurationManager.AppSettings));
        }

        [Test]
        public void Should_promote_custom_configuration_to_default()
        {
            try
            {
                NConfigurator.UsingFile("Configs\\NConfigTest.config").SetAsDefault();

                Assert.That(NConfigurator.Default.FileNames, Is.EqualTo(NConfigurator.UsingFile("Configs\\NConfigTest.config").FileNames));
                Assert.That(NConfigurator.Default.ConnectionStrings, Is.EqualTo(NConfigurator.UsingFile("Configs\\NConfigTest.config").ConnectionStrings));
                Assert.That(NConfigurator.Default.AppSettings, Is.EqualTo(NConfigurator.UsingFile("Configs\\NConfigTest.config").AppSettings));
            }
            finally
            {
                NConfigurator.UsingFiles().SetAsDefault(); // Restore default Default
            }

        }

        [Test]
        public void Should_promote_custom_configuration_to_system_default()
        {
            try
            {
                var connectStrings = NConfigurator.UsingFile("Configs\\NConfigTest.config").ConnectionStrings;
                var appSettings = NConfigurator.UsingFile("Configs\\NConfigTest.config").AppSettings;
                NConfigurator.UsingFile("Configs\\NConfigTest.config").SetAsSystemDefault();

                Assert.That(ConfigurationManager.ConnectionStrings, Is.EqualTo(connectStrings));
                Assert.That(ConfigurationManager.AppSettings, Is.EqualTo(appSettings));
            }
            finally
            {
                NConfigurator.RestoreSystemDefaults();
            }
        }

        [Test]
        public void Should_return_non_requied_section_instance()
        {
            var section = NConfigurator.UsingFile("Configs\\NConfigTest.config").GetSection("system.diagnostics");

            Assert.That(section, Is.Not.Null);
        }

        [Test]
        public void Should_call_section_merger_registered_for_section_type()
        {
            NSectionMerger<TestSection> mergerStub = MockRepository.GenerateStub<NSectionMerger<TestSection>>();
            mergerStub.Stub(m => m.Merge((IEnumerable<TestSection>)null)).IgnoreArguments().Return(null);

            try
            {
                NConfigurator.RegisterSectionMerger<TestSection>(mergerStub);
                var testSection = NConfigurator.UsingFiles("Configs\\Aliased.config", "Configs\\NConfigTest.config").GetSection<TestSection>();

                mergerStub.AssertWasCalled(m => m.Merge((IEnumerable<TestSection>)null), opt => opt.IgnoreArguments());
            }
            finally
            {
                NConfigurator.RegisterSectionMerger<TestSection>(new DefaultMerger());
            }

        }

        [Test]
        public void Should_pass_sections_to_section_merger_starting_from_most_important()
        {
            NSectionMerger<TestSection> mergerStub = MockRepository.GenerateStub<NSectionMerger<TestSection>>();

            // Just concatenate all section Values. 
            Func<IEnumerable<TestSection>, TestSection> mergeFunc = sections => new TestSection() { Value = sections.Aggregate("", (v, s) => v += s.Value) };
            mergerStub.Stub(m => m.Merge((IEnumerable<TestSection>)null)).IgnoreArguments().Do(mergeFunc);

            try
            {
                NConfigurator.RegisterSectionMerger(mergerStub);
                var testSection = NConfigurator.UsingFiles("Configs\\Aliased.config", "Configs\\NConfigTest.config").GetSection<TestSection>();

                //Concatenated values should be concatenated in order: from the most important section to lower.
                Assert.That(testSection.Value, Is.EqualTo("Tests.AliasedAliased"));
            }
            finally
            {
                NConfigurator.RegisterSectionMerger<TestSection>(new DefaultMerger());
            }
        }

        [Test]
        public void Should_return_IConfigurationSectionHandler_Create_call_result_for_legacy_sections()
        {
            object section = NConfigurator.UsingFile("Configs\\SectionHandler.config").GetSectionUntyped("TestSectionHandler");

            Assert.That(section, Is.EqualTo("TestSectionHandler Data"));
        }

        [Test]
        public void Should_return_null_when_legacy_section_declared_but_not_present()
        {
            var section = NConfigurator.UsingFile("Configs\\EmptySectionHandler.config").GetSectionUntyped("TestSectionHandler") as string;

            Assert.That(section, Is.Null);
        }

        [Test]
        public void Should_read_declared_but_not_present_section_from_custom_config_file()
        {

            var emptySection = NConfigurator.UsingFile("Configs\\NConfigTest.config").GetSection<TestSection>("EmptySection");

            Assert.That(emptySection, Is.Not.Null);
        }

        [Test]
        public void Should_read_defult_values_from_not_present_section_from_custom_config_file()
        {

            var emptySection = NConfigurator.UsingFile("Configs\\NConfigTest.config").GetSection<TestSection>("EmptySection");

            Assert.That(emptySection.Value, Is.EqualTo("DEFAULT"));
        }


        [Test]
        public void Should_dump_correct_diagnostics()
        {
            var diagnostics =
                NConfigurator.UsingFiles("NotExisting.config", "Configs\\Aliased.config").DumpDiagnostics();

            Assert.That(diagnostics, Is.StringContaining("NConfig Diagnostics"));
            Assert.That(diagnostics, Is.StringContaining("Host name: " + Environment.MachineName));
            Assert.That(diagnostics, Is.StringContaining("Host alias: Tests"));
            Assert.That(diagnostics, Is.StringContaining("Web Environment: False"));
            
            Assert.That(diagnostics, Is.StringContaining("missing file: 'Tests.NotExisting.config' location:"));
            Assert.That(diagnostics, Is.StringContaining("missing file: 'NotExisting.config' location:"));
            Assert.That(diagnostics, Is.StringContaining(@"exists file: 'Configs\Tests.Aliased.config' location:"));
            Assert.That(diagnostics, Is.StringContaining(@"exists file: 'Configs\Aliased.config' location:"));

        }

    
    }
}