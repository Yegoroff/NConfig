using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NConfig;
using System.Web.Configuration;
using System.Diagnostics;
using System.Web.WebPages.Scope;
using System.Reflection;

namespace NConfigMvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        static MvcApplication()
        {
            // This will simulate single core CPU so we can check how configuration substitution works on such systems.
            // System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)1;

            // We need to initialize NConfigurator once per AppDomain. Static ctor works well for this purpose.
            NConfigurator.UsingFile(@"~\Config\Custom.config").SetAsSystemDefault();
        }


        protected void Application_Start()
        {            
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}