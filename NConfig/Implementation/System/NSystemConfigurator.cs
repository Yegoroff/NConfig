using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Internal;

namespace NConfig
{
    /// <summary>
    /// Encapsulates forbidden magic that allows to smoothly use NConfig and System.Configuration
    /// </summary>
    internal class NSystemConfigurator: INSystemConfigurator
    {

        public IInternalConfigSystem OriginalConfiguration { get; protected set; }


        public virtual void SubstituteSystemConfiguration(IConfigurationFactory factory, IList<string> fileNames)
        {
            if (OriginalConfiguration != null)
                throw new InvalidOperationException("System default configuration already substituted.");

            OriginalConfiguration = SubstituteConfigurationSystem(factory, fileNames);
        }


        public virtual void RestoreSystemConfiguration()
        {
            if (OriginalConfiguration == null)
                return;

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));
            configManager.SetField("s_configSystem", OriginalConfiguration);
            OriginalConfiguration = null;
        }


        protected static IInternalConfigSystem SubstituteConfigurationSystem(IConfigurationFactory factory, IList<string> fileNames)
        {

            var configManager = new ReflectionAccessor(typeof(ConfigurationManager));

            ConfigurationManager.GetSection("appSettings"); // This will init Configuration manager internal config system.
            var originalConfigSystem = configManager.GetField<IInternalConfigSystem>("s_configSystem");

            if (originalConfigSystem is NSystemReplacementConfiguration)
                throw new InvalidOperationException("System default configuration already substituted.");

            NSystemReplacementConfiguration replacementConfigSytem = factory.CreateSystemReplacementConfiguration(originalConfigSystem, fileNames);

            configManager.SetField("s_configSystem", replacementConfigSytem);

            return originalConfigSystem;
        }

    }
}
