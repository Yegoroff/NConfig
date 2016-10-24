namespace NConfig
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;


    /// <summary>
    /// Generic sections merger which performs deep configuration merge.
    /// Important: Your collections need to implement <see cref="IMergeableConfigurationCollection"/>.
    /// </summary>
    /// <typeparam name="TSection">Configuration section type.</typeparam>
    public class DeepMerger<TSection> : NSectionMerger<TSection>
        where TSection : ConfigurationSection, new()
    {

        /// <summary>
        /// Merges the specified typed configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from the most important to lower.</param>
        /// <returns>The merge resulting section.</returns>
        public override TSection Merge(IEnumerable<TSection> sections)
        {
            if (sections == null || !sections.Any())
            {
                throw new ConfigurationErrorsException("No sections to merge");
            }

            var mergedSection = new TSection();

            DeepSectionMergerImpl.DoMerge(sections, mergedSection);

            return mergedSection;
        }
    }

    /// <summary>
    /// Non-generic sections merger which performs deep configuration merge.
    /// </summary>
    public class DeepMerger : NSectionMerger
    {

        /// <summary>
        /// Merges the specified typed configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from the most important to lower.</param>
        /// <returns>The merge resulting section.</returns>
        public override ConfigurationSection Merge(IEnumerable<ConfigurationSection> sections)
        {
            if (sections == null || !sections.Any())
            {
                throw new ConfigurationErrorsException("No sections to merge");
            }

            Type sectionType = sections.First().GetType();
            ConfigurationSection mergedSection = (ConfigurationSection) Activator.CreateInstance(sectionType);

            DeepSectionMergerImpl.DoMerge(sections, mergedSection);

            return mergedSection;
        }
    }


    /// <summary>
    ///   Used for implementing the generic and non-generic versions of the merger.
    /// </summary>
    internal static class DeepSectionMergerImpl
    {
        /// <summary>
        ///   Fill in mergedSection as a merger of sections.
        ///   Be carefull: in some circumstances one of sections may be modified.
        /// </summary>
        /// <param name="mergedSection">Modified to be the merger</param>
        /// <param name="sections">The sections, in order, to be merged.</param>
        internal static void DoMerge(IEnumerable<ConfigurationSection> sections, ConfigurationSection mergedSection)
        {
            foreach (PropertyInformation resultProperty in mergedSection.ElementInformation.Properties)
            {
                var propertiesOfSections = sections
                    .Select(section => section.ElementInformation.Properties[resultProperty.Name])
                    .ToList();
                UpdateProperty(propertiesOfSections, resultProperty);
            }
        }


        private static void UpdateProperty(List<PropertyInformation> sectionsProperties, PropertyInformation resultProperty)
        {
            var resultType = GetInheritance(resultProperty);

            switch (resultType)
            {

                case InheritanceTypes.COLLECTION:

                    if (!(resultProperty.Value is IMergeableConfigurationCollection))
                    {
                        throw new ConfigurationErrorsException(
                            $"Collection to merge {resultProperty.Name} is not of type {nameof(IMergeableConfigurationCollection)}. Necessary for the merger.");
                    }

                    var resultElementColl = (IMergeableConfigurationCollection) resultProperty.Value;
                    MergeCollection(sectionsProperties, resultElementColl);

                    // attributes of the collection i.e. http://stackoverflow.com/questions/8829414/how-to-have-custom-attribute-in-configurationelementcollection
                    MergeRecursive(sectionsProperties, resultProperty);

                    break;

                case InheritanceTypes.CONFIGURATION_ELEMENT:
                    MergeRecursive(sectionsProperties, resultProperty);

                    break;
                default:
                    // The order of sections should be from the most important to lower
                    // copied from property - merger
                    MergePropertyValue(sectionsProperties, resultProperty);
                    break;
            }
        }

        private enum InheritanceTypes
        {
            NONE,
            CONFIGURATION_ELEMENT,
            COLLECTION,
        }

        private static InheritanceTypes GetInheritance(PropertyInformation prop)
        {

            if (Inherits<ConfigurationElementCollection>(prop.Type))
            {
                //For us all are the same:
                // a) We do the sorting anyway, 
                // b) Basic does not allow remove/clear anyway, so both properties will never occur
                return InheritanceTypes.COLLECTION;
            }

            if (Inherits<ConfigurationElement>(prop.Type))
            {
                return InheritanceTypes.CONFIGURATION_ELEMENT;
            }
            return InheritanceTypes.NONE;
        }

        private static void MergeRecursive(List<PropertyInformation> sectionsProperties, PropertyInformation resultProperty)
        {
            var configElement = (ConfigurationElement) resultProperty.Value;
            foreach (PropertyInformation recursiveProperty in configElement.ElementInformation.Properties)
            {
                var propertiesOfSections = sectionsProperties
                    .Select(
                        prop =>
                            ((ConfigurationElement) prop.Value).ElementInformation.Properties[recursiveProperty.Name])
                    .ToList();
                UpdateProperty(propertiesOfSections, recursiveProperty);
            }
        }

        /// <summary>
        ///   Merges one property's value. Regarding the hierachy level the first set value is used as result.
        /// </summary>
        private static void MergePropertyValue(List<PropertyInformation> sectionsProperties, PropertyInformation resultProperty)
        {
            foreach (PropertyInformation sectionProperty in sectionsProperties)
            {
                if (sectionProperty == null)
                {
                    break;
                }
                var isDefinedHere = IsConfigurationPropertyDefined(sectionProperty);
                resultProperty.Value = sectionProperty.Value;
                if (isDefinedHere)
                {
                    // leave the value of first defined in config file property.
                    return;
                }
            }
        }

        /// <summary>
        ///   The sections are ordered from "highest" to "lowest".
        ///   if there is an :add: it is added in the section.
        ///   There may be a :remove: "higher" which can be caught by collection.IsRemoved()
        ///   The elements of a key are merged until there is another remove (or clear).
        ///   A :clear: is got by section.EmitClear and need to break the processing afterwards.
        ///
        ///   What to do
        ///   a) Cast and cut after clear
        ///   b) for each element to add, build up a list of ones to be merged
        ///   c) for each list, merge and add to result
        /// </summary>
        private static void MergeCollection(IEnumerable<PropertyInformation> sectionsProperties,
            IMergeableConfigurationCollection resultElementColl)
        {
            var collectionInSections = CutAfterClear(GetMergeableCollections(sectionsProperties)).ToArray();

            // key -> mergeList
            var allElements = new ConcurrentDictionary<object, List<ConfigurationElement>>();

            for (int i = 0; i < collectionInSections.Length; i++)
            {
                foreach (ConfigurationElement item in collectionInSections[i])
                {

                    if (IsRemoved(collectionInSections, item, i))
                    {
                        // there was a :remove: in higher order, so ignore this
                        // ignores also "later" elements after an :remove:
                        continue;
                    }

                    // Adds into list (or start a new one)
                    var key = collectionInSections[i].GetElementKeyForMerge(item);
                    var list = allElements.GetOrAdd(key, new List<ConfigurationElement>());
                    list.Add(item);
                }
            }

            // Do the merge
            foreach (var configElementList in allElements.Values)
            {
                ConfigurationElement mergedItem = MergeConfigurationElementList(configElementList);
                resultElementColl.Add(mergedItem);
            }
        }

        /// <summary>
        ///   Merges the configs and does the recursion.
        /// </summary>
        private static ConfigurationElement MergeConfigurationElementList(IEnumerable<ConfigurationElement> configs)
        {
            //A possible problem may be that we are overriding an element of a different config. for unmerging this is probably wrong
            //However, NConfig does not require us to be able to unmerge...
            var item = configs.First();

            foreach (PropertyInformation recursiveProperty in item.ElementInformation.Properties)
            {
                var propertiesOfSections = configs
                    .Select(prop => (prop).ElementInformation.Properties[recursiveProperty.Name])
                    .Where(prop => prop != null)
                    .ToList();
                UpdateProperty(propertiesOfSections, recursiveProperty);
            }
            return item;
        }

        /// <summary>
        ///   Was the item removed in any "upper" hierachy level?
        /// </summary>
        private static bool IsRemoved(IMergeableConfigurationCollection[] collectionInSections,
            ConfigurationElement item, int currentLevel)
        {
            for (int i = currentLevel - 1; i >= 0; i--)
            {
                if (collectionInSections[i].IsRemoved(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Ommit all collections lower an clear tag.
        /// </summary>
        private static IEnumerable<IMergeableConfigurationCollection> CutAfterClear(
            IEnumerable<IMergeableConfigurationCollection> untypedCollections)
        {
            foreach (var coll in untypedCollections)
            {
                yield return coll;
                // After yield return as current has to be still used
                if (coll.EmitClear)
                {
                    yield break;
                }
            }
        }

        /// <summary>
        ///   From all hierachy gets the property value casted as IMergeableConfigurationCollection.
        /// </summary>
        private static IEnumerable<IMergeableConfigurationCollection> GetMergeableCollections(
            IEnumerable<PropertyInformation> sectionsProperties)
        {
            return sectionsProperties
                .Where(x => x != null)
                .Select(sectionProperty => sectionProperty.Value)
                .Cast<IMergeableConfigurationCollection>(); // throws on purpose, so the user knows the problem
        }



        /// <summary>
        ///   Does type inherit from TBase?
        /// </summary>
        private static bool Inherits<TBase>(Type type) where TBase : class
        {
            Type cur = type;

            while (cur != null)
            {
                if (cur == typeof(TBase))
                {
                    return true;
                }

                cur = cur.BaseType;
            }

            return false;
        }

        private static bool IsConfigurationPropertyDefined(PropertyInformation property)
        {
            return property.ValueOrigin == PropertyValueOrigin.SetHere;
        }
    }
}
