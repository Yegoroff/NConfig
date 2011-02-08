using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NConfig
{
    /// <summary>
    /// Default Configuration section merger, that just return first section among provided.
    /// </summary>
    public sealed class DefaultMerger : NSectionMerger
    {

        /// <summary>
        /// Merges the specified configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from most important to lower.</param>
        /// <returns>The merge result section.</returns>
        public override ConfigurationSection Merge(IEnumerable<ConfigurationSection> sections)
        {
            return sections.FirstOrDefault();
        }
    }
}
