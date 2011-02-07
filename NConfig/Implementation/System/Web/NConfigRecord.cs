using System.Configuration.Internal;
namespace NConfig
{
    internal class NConfigRecord : IInternalConfigRecord
    {
        private readonly IInternalConfigRecord baseRecord;


        public NConfigRecord(IInternalConfigRecord baseRecord)
        {
            this.baseRecord = baseRecord;
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
            return baseRecord.GetSection(configKey);
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
