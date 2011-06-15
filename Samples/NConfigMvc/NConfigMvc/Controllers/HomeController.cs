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
            var appSettings = (from object s in ConfigurationManager.AppSettings select s.ToString()).ToList();

            string[] appSettingsExpected = { "ExtendedParam", "ClientValidationEnabled", "UnobtrusiveJavaScriptEnabled", "RootParam" };
            Debug.Assert(appSettings.SequenceEqual(appSettingsExpected), "Invalid Application Settings");

            ViewBag.Message = " Application Settings: " + string.Join(", ", appSettings);

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
