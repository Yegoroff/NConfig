using System.Configuration.Internal;

namespace NConfig
{
    /// <summary>
    /// NConfig IInternalConfigRecord decorator, that adds custom configurations to web system default configuration.
    /// </summary>
    internal class NConfigRecordReplacement : IInternalConfigRecord
    {
        private readonly IInternalConfigRecord originalRecord;
        private readonly INConfiguration configuration;

        public NConfigRecordReplacement(IInternalConfigRecord originalRecord, INConfiguration configuration)
        {
            this.originalRecord = originalRecord;
            this.configuration = configuration;
        }


        #region IInternalConfigRecord Members


        public string ConfigPath
        {
            get
            {
                return originalRecord.ConfigPath;
            }
        }

        public bool HasInitErrors
        {
            get
            {
                return originalRecord.HasInitErrors;
            }
        }

        public string StreamName
        {
            get
            {
                return originalRecord.StreamName;
            }
        }


        public object GetLkgSection(string configKey)
        {
            return originalRecord.GetLkgSection(configKey);
        }

        public object GetSection(string configKey)
        {
            return configuration.GetSectionUntyped(configKey);
        }

        public void RefreshSection(string configKey)
        {
            originalRecord.RefreshSection(configKey);
        }

        public void Remove()
        {
            originalRecord.Remove();
        }

        public void ThrowIfInitErrors()
        {
            originalRecord.ThrowIfInitErrors();
        }

        #endregion
    }

}
