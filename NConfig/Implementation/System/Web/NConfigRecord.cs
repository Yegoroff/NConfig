using System.Configuration.Internal;
namespace NConfig
{
    internal class NConfigRecord : IInternalConfigRecord
    {
        private readonly IInternalConfigRecord baseRecord;
        private readonly INConfiguration configuration;

        private readonly object syncObject = new object();
        private bool isEntered;

        public NConfigRecord(IInternalConfigRecord baseRecord, INConfiguration configuration)
        {
            this.baseRecord = baseRecord;
            this.configuration = configuration;
        }


        #region IInternalConfigRecord Members


        public string ConfigPath
        {
            get
            {
                return baseRecord.ConfigPath;
            }
        }

        public bool HasInitErrors
        {
            get
            {
                return baseRecord.HasInitErrors;
            }
        }

        public string StreamName
        {
            get
            {
                return baseRecord.StreamName;
            }
        }


        public object GetLkgSection(string configKey)
        {
            return baseRecord.GetLkgSection(configKey);
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
                            result = configuration.GetSection(configKey);

                            if (result == null)
                                return baseRecord.GetSection(configKey);
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
            baseRecord.RefreshSection(configKey);
        }

        public void Remove()
        {
            baseRecord.Remove();
        }

        public void ThrowIfInitErrors()
        {
            baseRecord.ThrowIfInitErrors();
        }

        #endregion
    }

}
