using System.Configuration.Internal;
using System;
using System.Collections.Generic;

namespace NConfig
{
    internal class NConfigSystemReplacement : IConfigSystem
    {
        private readonly IConfigSystem originalConfigSystem;
        private readonly NConfigRootReplacement replacedRoot;


        public NConfigSystemReplacement(IConfigSystem originalConfigSystem, IConfigurationFactory factory, IList<string> fileNames)
        {
            this.originalConfigSystem = originalConfigSystem;
            replacedRoot = new NConfigRootReplacement(originalConfigSystem.Root, factory, fileNames);
        }


        #region IConfigSystem Members

        public IInternalConfigHost Host
        {
            get {
                return originalConfigSystem.Host;
            }
        }

        public void Init(Type typeConfigHost, params object[] hostInitParams)
        {
            originalConfigSystem.Init(typeConfigHost, hostInitParams);
        }

        public IInternalConfigRoot Root
        {
            get {
                return replacedRoot;    
            }
        }

        #endregion
    }

}
