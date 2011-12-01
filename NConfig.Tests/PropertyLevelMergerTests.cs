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
    public class PropertyLevelMergerTests
    {

        [Test]
        public void Should_return_default_values_when_only_empty_section_provided()
        {
            var mergerUnderTest = new PropertyMerger<MergeSection>();
            var emptySection = NConfigurator.UsingFile("Configs\\MergeTest.config").GetSection<MergeSection>("EmptySection");

            var resultSection = mergerUnderTest.Merge(new[]{emptySection});

            Assert.That(resultSection.IntValue, Is.EqualTo(1234));
            Assert.That(resultSection.BoolValue, Is.EqualTo(true));
            Assert.That(resultSection.StringValue, Is.EqualTo("DEFAULT"));
        }

        [Test]
        public void Should_return_definded_values_when_only_filled_section_provided()
        {
            var mergerUnderTest = new PropertyMerger<MergeSection>();
            var fullSection = NConfigurator.UsingFile("Configs\\MergeTest.config").GetSection<MergeSection>("FullSection");

            var resultSection = mergerUnderTest.Merge(new[] { fullSection });

            Assert.That(resultSection.IntValue, Is.EqualTo(4321));
            Assert.That(resultSection.BoolValue, Is.EqualTo(false));
            Assert.That(resultSection.StringValue, Is.EqualTo("FULL"));
        }


        [Test]
        public void Should_return_overrided_values_when_several_sections_provided()
        {
            var mergerUnderTest = new PropertyMerger<MergeSection>();
            var fullSection = NConfigurator.UsingFile("Configs\\MergeTest.config").GetSection<MergeSection>("FullSection");
            var overrideSection = NConfigurator.UsingFile("Configs\\MergeTest.config").GetSection<MergeSection>("OverrideSection");

            var resultSection = mergerUnderTest.Merge(new[] { overrideSection ,fullSection });

            Assert.That(resultSection.IntValue, Is.EqualTo(4321));
            Assert.That(resultSection.BoolValue, Is.EqualTo(true));
            Assert.That(resultSection.StringValue, Is.EqualTo("OVERRIDE"));
        }

        [Test]
        public void Should_not_override_with_default_values_when_several_sections_provided()
        {
            var mergerUnderTest = new PropertyMerger<MergeSection>();
            var fullSection = NConfigurator.UsingFile("Configs\\MergeTest.config").GetSection<MergeSection>("FullSection");
            var emptySection = NConfigurator.UsingFile("Configs\\MergeTest.config").GetSection<MergeSection>("EmptySection");

            var resultSection = mergerUnderTest.Merge(new[] { emptySection, fullSection });

            Assert.That(resultSection.IntValue, Is.EqualTo(4321));
            Assert.That(resultSection.BoolValue, Is.EqualTo(false));
            Assert.That(resultSection.StringValue, Is.EqualTo("FULL"));
        }

    }


}

