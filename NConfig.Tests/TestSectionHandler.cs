using System.Configuration;
using System.Xml;

namespace NConfig.Tests
{
    public class TestSectionHandler : IConfigurationSectionHandler
    {
        #region Implementation of IConfigurationSectionHandler

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.InnerText;
        }

        #endregion
    }
}
