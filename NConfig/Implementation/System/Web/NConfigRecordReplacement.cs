using System.Configuration.Internal;
using System.Diagnostics;
namespace NConfig
{
    internal class NConfigRecordReplacement : IInternalConfigRecord
    {
        private readonly IInternalConfigRecord originalRecord;
        private readonly INConfiguration configuration;

        private readonly object syncObject = new object();
        private bool isEntered;

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
            object result = null;

            if (!isEntered)
            {
                lock (syncObject)
                {
                    if (!isEntered)
                    {
                        try
                        {
                            isEntered = true;
                            return configuration.GetSectionUntyped(configKey);
                        }
                        finally
                        {
                            isEntered = false;
                        }
                    }
                }
            }
            return result;
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
