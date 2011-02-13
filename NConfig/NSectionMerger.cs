using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace NConfig
{
    /// <summary>
    /// Provides functionality to merge configuration sections from different configuration files.
    /// </summary>
    public abstract class NSectionMerger
    {
        /// <summary>
        /// Merges the specified configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from the most important to lower.</param>
        /// <returns>The merge result section.</returns>
        public abstract ConfigurationSection Merge(IEnumerable<ConfigurationSection> sections);
    }


    /// <summary>
    /// Provides typed functionality to merge configuration sections from different configuration files.
    /// </summary>
    /// <typeparam name="T">The ConfigurationSection type.</typeparam>
    public abstract class NSectionMerger<T> : NSectionMerger where T : ConfigurationSection
    {

        /// <summary>
        /// Merges the specified configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from the most important to lower.</param>
        /// <returns>The merge resulting section.</returns>
        public override sealed ConfigurationSection Merge(IEnumerable<ConfigurationSection> sections)
        {
            return Merge(sections.Cast<T>());
        }

        /// <summary>
        /// Merges the specified typed configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from the most important to lower.</param>
        /// <returns>The merge resulting section.</returns>
        public abstract T Merge(IEnumerable<T> sections);
    }
}
