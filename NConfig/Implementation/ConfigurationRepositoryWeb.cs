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
            // Check if file path is full physical local path.
            if (Path.GetFullPath(path) == path) 
                return path;

            if (!VirtualPathUtility.IsAbsolute(path) && !VirtualPathUtility.IsAppRelative(path))
                path = VirtualPathUtility.ToAppRelative("\\" + path);

            return System.Web.Hosting.HostingEnvironment.MapPath(path);
        }

    }
}
