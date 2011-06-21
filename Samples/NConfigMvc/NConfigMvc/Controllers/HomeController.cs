using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace NConfigMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var appSettings = (from object s in ConfigurationManager.AppSettings select s.ToString()).ToList();

            string[] appSettingsExpected = { "ExtendedParam", "ClientValidationEnabled", "UnobtrusiveJavaScriptEnabled", "RootParam" };
            
            if (!appSettings.SequenceEqual(appSettingsExpected))
                ViewBag.Error = "Invalid Application Settings";

            ViewBag.Message = "Application Settings: " + string.Join(", ", appSettings);

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
