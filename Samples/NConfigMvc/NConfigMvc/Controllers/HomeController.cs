using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using NConfig;

namespace NConfigMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var testSection = NConfigurator.Default.GetSection<TestConfigSection>();

            var configManagerTestSection = ConfigurationManager.GetSection("TestConfigSection") as TestConfigSection;
            
            var namedTestSection = NConfigurator.UsingFile(@"Config\Custom.config").GetSection<TestConfigSection>("NamedSection");

            ViewBag.NConfigDefault = testSection.TestValue;
            ViewBag.ConfigurationManager = configManagerTestSection.TestValue;
            ViewBag.NConfigNamed = namedTestSection.TestValue;

            ViewBag.AppSettings = new List<string>(); 
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                ViewBag.AppSettings.Add(key + " : " + ConfigurationManager.AppSettings[key]);
            }

            ViewBag.ConnectionStrings = new List<string>(); 
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                ViewBag.ConnectionStrings.Add(connectionString.Name + " : " + connectionString.ConnectionString);
            }

            return View();
        }
    }

    static class StringExtensions
    {
        public static string F(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }

}
