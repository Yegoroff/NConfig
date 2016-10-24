using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NConfig.Tests {
    [TestFixture]
    public class DeepMergerTests {
        private const string FOLDER = "DeepMergeConfigs\\";
        private const string CONFIG_1 = FOLDER + "Test1.config";
        private const string CONFIG_2 = FOLDER + "Test2.config";
        private const string CONFIG_3 = FOLDER + "Test3.config";
        private const string CONFIG_4 = FOLDER + "Test4.config";

        #region GetMergedConfig
        private IEnumerable<FileInfo> GetFiles() {
            yield return new FileInfo(CONFIG_1);
            yield return new FileInfo(CONFIG_2);
            yield return new FileInfo(CONFIG_3);
            yield return new FileInfo(CONFIG_4);
        }

        private DeepMergerTestSection GetMergedConfig() {
            var merger = new DeepMerger<DeepMergerTestSection>();
            NConfigurator.RegisterSectionMerger(merger);

            IEnumerable<FileInfo> files = GetFiles();
            string[] configFileNames = files
                    .Select(x => x.FullName)
                    .ToArray();
            return NConfigurator
                    .UsingFiles(configFileNames)
                    .GetSection<DeepMergerTestSection>("mergeTest");
        }
        #endregion

        [Test]
        public void DeepMerger_MergeProperties() {
            DeepMergerTestSection testSection = GetMergedConfig();

            Assert.IsNotNull(testSection);
            Assert.AreEqual("override2", testSection.OverriddenProperty.Value);
            Assert.AreEqual("override", testSection.JumpOverProperty.Value);
            Assert.AreEqual("", testSection.NewEmptyProperty.Value);
            Assert.AreEqual("test", testSection.NewNullProperty.Value);
        }

        [Test]
        public void DeepMerger_MergeCollections_ClearCollection() {
            DeepMergerTestSection testSection = GetMergedConfig();
            
            var coll = testSection?.ClearCollection;
            var test1 = coll?.FirstOrDefault(x => x.Name == "test1");
            var test2 = coll?.FirstOrDefault(x => x.Name == "test2");
            var test3 = coll?.FirstOrDefault(x => x.Name == "test3");
            var test4 = coll?.FirstOrDefault(x => x.Name == "test4");

            Assert.IsNotNull(testSection);
            Assert.IsNotNull(coll);
            Assert.AreEqual(3, coll.Count);
            
            Assert.IsNotNull(test1);
            Assert.AreEqual("test", test1.Value);

            Assert.IsNotNull(test2);
            Assert.AreEqual("", test2.Value); // interestingly not null

            Assert.IsNull(test3);

            Assert.IsNotNull(test4);
            Assert.AreEqual("test", test4.Value);
        }

        [Test]
        public void DeepMerger_MergeCollections_RemoveCollection() {
            DeepMergerTestSection testSection = GetMergedConfig();

            var coll = testSection?.RemoveCollection;
            var test1 = coll?.FirstOrDefault(x => x.Name == "removeSame");
            var test2 = coll?.FirstOrDefault(x => x.Name == "fullRemove");
            var test3 = coll?.FirstOrDefault(x => x.Name == "fullRemoveLower");
            var test4 = coll?.FirstOrDefault(x => x.Name == "removedLower");
            var test5 = coll?.FirstOrDefault(x => x.Name == "removedLowerWithSpace");

            Assert.IsNotNull(testSection);
            Assert.IsNotNull(coll);
            Assert.AreEqual(3, coll.Count);

            Assert.IsNotNull(test1);
            Assert.AreEqual("test", test1.Value);

            Assert.IsNull(test2);
            Assert.IsNull(test3);

            Assert.IsNotNull(test4);
            Assert.AreEqual("test", test4.Value);

            Assert.IsNotNull(test5);
            Assert.AreEqual("test", test5.Value);
        }

        [Test]
        public void DeepMerger_MergeCollections_SubCollection_Merged() {
            DeepMergerTestSection testSection = GetMergedConfig();

            var coll = testSection?.SubCollection;
            var test1Coll = coll?.FirstOrDefault(x => x.Name == "test1");
            var test2Coll = coll?.FirstOrDefault(x => x.Name == "test2");
            var test3Coll = coll?.FirstOrDefault(x => x.Name == "test3");

            var test1T1 = test1Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t1");
            var test1T2 = test1Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t2");
            var test1T3 = test1Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t3");
            var test1T4 = test1Coll?.SubCollection?.FirstOrDefault(x => x.Name == "removed");

            // The collections are all there
            Assert.IsNotNull(testSection);
            Assert.IsNotNull(coll);
            Assert.IsNotNull(test1Coll);
            Assert.IsNotNull(test2Coll);
            Assert.IsNotNull(test3Coll);

            Assert.IsNotNull(test1T1);
            Assert.AreEqual("test", test1T1.Value);

            Assert.IsNotNull(test1T2);
            Assert.AreEqual("override", test1T2.Value);

            Assert.IsNotNull(test1T3);
            Assert.AreEqual("test", test1T3.Value);

            Assert.IsNull(test1T4); // removed
        }

        [Test]
        public void DeepMerger_MergeCollections_SubCollection_NoSideEffects() {
            DeepMergerTestSection testSection = GetMergedConfig();

            var coll = testSection?.SubCollection;
            var test1Coll = coll?.FirstOrDefault(x => x.Name == "test1");
            var test2Coll = coll?.FirstOrDefault(x => x.Name == "test2");
            var test3Coll = coll?.FirstOrDefault(x => x.Name == "test3");

            var test2T1 = test2Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t1");
            var test2T2 = test2Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t2");
            //var test2T3 = test2Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t3");
            var test2T4 = test2Coll?.SubCollection?.FirstOrDefault(x => x.Name == "removed");
            

            // The collections are all there
            Assert.IsNotNull(testSection);
            Assert.IsNotNull(coll);
            Assert.IsNotNull(test1Coll);
            Assert.IsNotNull(test2Coll);
            Assert.IsNotNull(test3Coll);

            Assert.IsNotNull(test2T1);
            Assert.AreEqual("test2", test2T1.Value);

            Assert.IsNull(test2T2);

            Assert.IsNull(test2T4);
        }

        [Test]
        public void DeepMerger_MergeCollections_SubCollection_Hopping() {
            DeepMergerTestSection testSection = GetMergedConfig();

            var coll = testSection?.SubCollection;
            var test3Coll = coll?.FirstOrDefault(x => x.Name == "test3");

            var test3T1 = test3Coll?.SubCollection?.FirstOrDefault(x => x.Name == "t1");

            // The collections are all there
            Assert.IsNotNull(testSection);
            Assert.IsNotNull(coll);
            Assert.IsNotNull(test3Coll);

            Assert.IsNotNull(test3T1);
            Assert.AreEqual("test4", test3T1.Value);
        }
    }

    #region DeepMergerTestSection

    public class DeepMergerTestSection : ConfigurationSection {
        [ConfigurationProperty("ClearCollection", IsDefaultCollection = false)]
        public TestElementCollectionElement ClearCollection => base["ClearCollection"] as TestElementCollectionElement;

        [ConfigurationProperty("RemoveCollection", IsDefaultCollection = false)]
        public TestElementCollectionElement RemoveCollection => base["RemoveCollection"] as TestElementCollectionElement;

        [ConfigurationProperty("SubCollection", IsDefaultCollection = false)]
        public TestCollectionElement SubCollection => base["SubCollection"] as TestCollectionElement;

        #region MergedProperties
        [ConfigurationProperty("OverriddenProperty", IsDefaultCollection = false)]
        public StringElement OverriddenProperty => base["OverriddenProperty"] as StringElement;

        [ConfigurationProperty("JumpOverProperty", IsDefaultCollection = false)]
        public StringElement JumpOverProperty => base["JumpOverProperty"] as StringElement;

        [ConfigurationProperty("NewEmptyProperty", IsDefaultCollection = false)]
        public StringElement NewEmptyProperty => base["NewEmptyProperty"] as StringElement;

        [ConfigurationProperty("NewNullProperty", IsDefaultCollection = false)]
        public TestElement NewNullProperty => base["NewNullProperty"] as TestElement;
        #endregion
    }

    public class StringElement : ConfigurationElement {
        [ConfigurationProperty("value", IsRequired = true, IsKey = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        public override string ToString() {
            return Value ?? "null";
        }
    }

    [ConfigurationCollection(typeof(TestElement), AddItemName = "ele",
            CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class TestElementCollectionElement : MergeableConfigurationCollection<TestElement> {
    }

    [ConfigurationCollection(typeof(SubElements), AddItemName = "main",
            CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class TestCollectionElement : MergeableConfigurationCollection<SubElements> {
    }

    public class SubElements : ConfigurationElement {

        [ConfigurationProperty("name", IsRequired = false, IsKey = true)]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("values", IsRequired = false, IsKey = false)]
        public TestSubCollectionElement SubCollection => base["values"] as TestSubCollectionElement;
        
        public HashSet<string> Subs => new HashSet<string>(SubCollection.Select(sub => sub.Value));

        public override string ToString() {
            var roles = string.Join("', '", Subs);
            return $"User: {Name}, Values: ('{roles}'), " + base.ToString();
        }
    }



    [ConfigurationCollection(typeof(TestElement), AddItemName = "sub",
            CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class TestSubCollectionElement : MergeableConfigurationCollection<TestElement> {

    }


    public class TestElement : ConfigurationElement {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = false, IsKey = false)]
        public string Value {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        public override string ToString() {
            return $"{Name} :: {Value}";
        }
    }
    #endregion
}
