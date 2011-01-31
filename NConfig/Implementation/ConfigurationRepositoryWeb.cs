using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace NConfig
{

    /// <summary>
    /// Provides web specific configuration file location.
    /// </summary>
    internal class ConfigurationRepositoryWeb : ConfigurationRepository
    {

        protected override string ToAbsolutePath(string path)
        {
            if (!VirtualPathUtility.IsAbsolute(path))
                path = VirtualPathUtility.ToAppRelative("\\" + path);

            return System.Web.Hosting.HostingEnvironment.MapPath(path);
        }

    }
}
