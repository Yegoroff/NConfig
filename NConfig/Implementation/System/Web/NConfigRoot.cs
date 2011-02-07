using System.Configuration.Internal;

namespace NConfig
{
    internal class NConfigRoot : IInternalConfigRoot
    {
        private readonly IInternalConfigRoot baseRoot;


        public NConfigRoot(IInternalConfigRoot baseRoot)
        {
            this.baseRoot = baseRoot;
        }


        #region IInternalConfigRoot Members

        /// <summary>
        /// We do not provide correct event decoration,
        /// when we need to took all the subscribers and resubscribe on our event instance, thus we provide corect instance in event args.
        /// Currently all noticed subscribers in FCL do not use args or sender, so we just notify them that something changed.
        /// </summary>
        public event InternalConfigEventHandler ConfigChanged
        {
            add
            {
                baseRoot.ConfigChanged += value;
            }
            remove
            {
                baseRoot.ConfigChanged -= value;
            }
        }

        public event InternalConfigEventHandler ConfigRemoved
        {
            add
            {
                baseRoot.ConfigRemoved += value;
            }
            remove
            {
                baseRoot.ConfigRemoved += value;
            }
        }


        public bool IsDesignTime
        {
            get {
                return baseRoot.IsDesignTime;
            }
        }


        public IInternalConfigRecord GetConfigRecord(string configPath)
        {
            //TODO: Consider memorization like IInternalConfigRecord -> NConfigRecord.
            return new NConfigRecord(baseRoot.GetConfigRecord(configPath));
        }

        public object GetSection(string section, string configPath)
        {
            return baseRoot.GetSection(section, configPath);
        }

        public string GetUniqueConfigPath(string configPath)
        {
            return baseRoot.GetUniqueConfigPath(configPath);
        }

        public IInternalConfigRecord GetUniqueConfigRecord(string configPath)
        {
            //TODO:  Consider memorization like IInternalConfigRecord -> NConfigRecord.
            return new NConfigRecord(baseRoot.GetUniqueConfigRecord(configPath));
        }

        public void Init(IInternalConfigHost host, bool isDesignTime)
        {
            baseRoot.Init(host, isDesignTime);
        }



        public void RemoveConfig(string configPath)
        {
            //TODO: This could affect memorization.
            baseRoot.RemoveConfig(configPath);
        }

        #endregion
    }

}
