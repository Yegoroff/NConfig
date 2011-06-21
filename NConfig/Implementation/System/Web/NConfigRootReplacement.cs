using System.Configuration.Internal;
using System.Collections.Generic;

namespace NConfig
{
    internal class NConfigRootReplacement : IInternalConfigRoot
    {
        private readonly IInternalConfigRoot originalRoot;
        private readonly IConfigurationFactory factory;
        private readonly IList<string> fileNames;
        

        public NConfigRootReplacement(IInternalConfigRoot originalRoot, IConfigurationFactory factory, IList<string> fileNames)
        {
            this.originalRoot = originalRoot;
            this.factory = factory;
            this.fileNames = fileNames;
        }


        public IInternalConfigRecord CreateConfigRecord(IInternalConfigRecord originalRecord)
        {
            //TODO: Consider caching like IInternalConfigRecord -> NConfigRecord.(Threadsafe)
            INConfiguration recordConfiguration = factory.CreateConfigRecordConfiguration(originalRecord, fileNames);
            return new NConfigRecordReplacement(originalRecord, recordConfiguration);
        }

        #region IInternalConfigRoot Members

        /// <summary>
        /// We do not provide correct event decoration, when we must take all the subscribers and 
        /// resubscribe on our event instances, thus we provide corect instance in event args.
        /// Currently all noticed subscribers in FCL do not use args or sender, so we just notify them that something changed.
        /// </summary>
        public event InternalConfigEventHandler ConfigChanged
        {
            add
            {
                originalRoot.ConfigChanged += value;
            }
            remove
            {
                originalRoot.ConfigChanged -= value;
            }
        }

        public event InternalConfigEventHandler ConfigRemoved
        {
            add
            {
                originalRoot.ConfigRemoved += value;
            }
            remove
            {
                originalRoot.ConfigRemoved -= value;
            }
        }


        public bool IsDesignTime
        {
            get {
                return originalRoot.IsDesignTime;
            }
        }


        public IInternalConfigRecord GetConfigRecord(string configPath)
        {

            return CreateConfigRecord(originalRoot.GetConfigRecord(configPath));
        }

        public object GetSection(string section, string configPath)
        {
            return originalRoot.GetSection(section, configPath);
        }

        public string GetUniqueConfigPath(string configPath)
        {
            return originalRoot.GetUniqueConfigPath(configPath);
        }

        public IInternalConfigRecord GetUniqueConfigRecord(string configPath)
        {
            return CreateConfigRecord(originalRoot.GetUniqueConfigRecord(configPath));
        }

        public void Init(IInternalConfigHost host, bool isDesignTime)
        {
            originalRoot.Init(host, isDesignTime);
        }



        public void RemoveConfig(string configPath)
        {
            //TODO: This could affect cache.
            originalRoot.RemoveConfig(configPath);
        }

        #endregion
    }

}
