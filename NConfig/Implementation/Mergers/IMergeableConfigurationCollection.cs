using System.Collections;
using System.Configuration;

namespace NConfig
{
    /// <summary>
    ///   Mergeable configuration collection (still untyped).
    ///   Is an IEnumerable(ConfigurationElement)
    /// </summary>
    public interface IMergeableConfigurationCollection : IEnumerable {
        /// <summary>
        ///   Put the element into the collection (used by merger).
        /// </summary>
        /// <param name="element">The element to put into.</param>
        void Add(ConfigurationElement element);
        //{ BaseAdd(element, false); }

        /// <summary>
        ///   Was this item removed &lt;remove6gt; in this configuration? (used by merger)
        /// </summary>
        bool IsRemoved(ConfigurationElement item);
        //{ return BaseIsRemoved(GetElementKey(item)); }

        /// <summary>
        ///   Gets the element key of item (used by merge)
        /// </summary>
        object GetElementKeyForMerge(ConfigurationElement item);
        //{ return GetElementKey(item); }

        /// <summary>
        ///   In this merge hierachy there was an clear tag, so that all lower hierachy members have to be ommitted.
        /// </summary>
        bool EmitClear { get; }
    }
}