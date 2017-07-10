using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NConfig
{
    /// <summary>
    ///   ConfigurationCollection having typed indexer and enumerator.
    /// </summary>
    /// <typeparam name="T">Type of the configuration element having a key.</typeparam>
    public abstract class MergeableConfigurationCollection<T> : ConfigurationElementCollection,
        IMergeableConfigurationCollection, IEnumerable<T>
        where T : ConfigurationElement, new()
    {

        /// <summary>
        ///   As we know the type of the element, we can create it.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        /// <summary>
        ///   Access to the i-th element type-safe.
        /// </summary>
        public T this[int index]
        {
            get { return BaseGet(index) as T; }
        }

        /// <summary>
        ///   Type-safe getter for the element with the key.
        /// </summary>
        public T GetElement(string key)
        {
            return BaseGet(key) as T;
        }

        /// <summary>
        ///   Enumerates all elements of the collection. 
        ///   Please note that the sort order is not defined.
        /// </summary>
        public new IEnumerator<T> GetEnumerator()
        {
            return (
                from i in Enumerable.Range(0, Count)
                select this[i]).GetEnumerator();
        }

        /// <summary>
        /// if T simply returns the value, no duplicates will be added to collection
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var keyElements = element.ElementInformation.Properties
                .Cast<PropertyInformation>()
                .Where(x => x.IsKey)
                .ToList();

            if (!keyElements.Any())
            {
                // no property is set as IsKey
                throw new ConfigurationErrorsException(
                    string.Format("Element type {0} in collection {1} does not have IsKey property specified: {2}",
                        element.GetType(), GetType(), element));
            }

            if (keyElements.Count == 1)
            {
                return keyElements.First().Value;
            }

            // For several IsKey = true properties: use an aggregated key so that they are compared together
            return string.Join(":--:", keyElements.Select(x => x.Value.ToString()).ToArray() );
        }

        void IMergeableConfigurationCollection.Add(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        bool IMergeableConfigurationCollection.IsRemoved(ConfigurationElement item)
        {
            return BaseIsRemoved(GetElementKey(item));
        }

        object IMergeableConfigurationCollection.GetElementKeyForMerge(ConfigurationElement item)
        {
            return GetElementKey(item);
        }
    }
}
