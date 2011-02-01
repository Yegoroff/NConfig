using System.Collections.Generic;
using System.Configuration;

namespace NConfig
{
    internal class ConnectionStringsMerger : NSectionMerger<ConnectionStringsSection>
    {

        public override ConnectionStringsSection Merge(IEnumerable<ConnectionStringsSection> sections)
        {
            ConnectionStringsSection result = new ConnectionStringsSection();

            foreach (ConnectionStringsSection section in sections)
            {
                foreach (ConnectionStringSettings settings in section.ConnectionStrings)
                    if (result.ConnectionStrings[settings.Name] == null)
                        result.ConnectionStrings.Add(settings);
            }

            return result;
        }
    }
}
