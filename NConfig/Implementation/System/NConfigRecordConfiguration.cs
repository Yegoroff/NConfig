using System.Collections.Generic;
using System.Configuration.Internal;

namespace NConfig
{
    internal sealed class NConfigRecordConfiguration : NMultifileConfiguration
    {
        private readonly IInternalConfigRecord configRecord;


        public NConfigRecordConfiguration(IInternalConfigRecord configRecord, IConfigurationRepository repository, INSectionMergerRegistry mergerRegistry, IList<string> fileNames) :
            base(repository, mergerRegistry, fileNames)
        {
            this.configRecord = configRecord;
        }

        protected override object GetAppWebSection(string sectionName)
        {
            return configRecord.GetSection(sectionName);
        }
    }
}
