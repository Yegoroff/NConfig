using System.Collections.Generic;
using System.Configuration;

namespace NConfig
{
    internal class AppSettingsMerger : NSectionMerger<AppSettingsSection>
    {

        public override AppSettingsSection Merge(IEnumerable<AppSettingsSection> sections)
        {
            AppSettingsSection result = new AppSettingsSection();

            foreach (AppSettingsSection section in sections) // The order of files shoud be from most Important to lower
            {
                foreach (KeyValueConfigurationElement element in section.Settings)
                    if (result.Settings[element.Key] == null)
                        result.Settings.Add(element);
            }

            return result;
        }
    }
}
