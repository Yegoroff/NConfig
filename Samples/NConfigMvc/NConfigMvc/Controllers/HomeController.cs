using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.WebPages.Razor.Configuration;
using System.Web.Configuration;
using NConfig;
using System.Diagnostics;

namespace NConfigMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string[] appSettingsExpected = {"ExtendedParam", "ClientValidationEnabled", "UnobtrusiveJavaScriptEnabled", "RootParam"};

            List<string> appSettings = new List<string>();
            foreach (var s in ConfigurationManager.AppSettings)
            {
                appSettings.Add(s.ToString());
            }

            Debug.Assert(appSettings.SequenceEqual(appSettingsExpected), "Invalid Application Settings");

            ViewBag.Message = "Application Settings: " + string.Join(", ", appSettings);

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
