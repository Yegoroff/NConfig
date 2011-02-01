using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NConfig
{
    /// <summary>
    /// Default COnfiguration section merger, that just return first section among provided.
    /// </summary>
    public sealed class DefaultMerger : NSectionMerger
    {

        public override ConfigurationSection Merge(IEnumerable<ConfigurationSection> sections)
        {
            return sections.FirstOrDefault();
        }
    }
}
