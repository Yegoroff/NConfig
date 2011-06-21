using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using NConfig;

namespace WebClient.Nest
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var errorMessages = new List<string>();

            var appSettingsExpected = new List<string> { "extendedParam", "rootParam", "nestedParam" };
            var connStringsExpected = new List<string> { "LocalSqlServer", "ExtendedConnection", "ApplicationServices", "RootConnection", "NestedConnection" };

            var appSettings = (from object item in ConfigurationManager.AppSettings select item.ToString()).ToList();

            var ncAppSettings = (from object item in NConfigurator.Default.AppSettings select item.ToString()).ToList();
            

            if (!appSettings.SequenceEqual(appSettingsExpected))
                errorMessages.Add("Invalid Application Settings");

            if (!ncAppSettings.SequenceEqual(appSettingsExpected))
                errorMessages.Add("Invalid NConfig Application Settings");

            var connStrings = (from ConnectionStringSettings item in ConfigurationManager.ConnectionStrings select item.Name).ToList();

            if (!connStrings.SequenceEqual(connStringsExpected))
                errorMessages.Add("Invalid Connection Strings");

            errors.InnerText = string.Join(", ", errorMessages);
            applicationSettings.InnerText = string.Join(", ", appSettings);
            connectionStrings.InnerText = string.Join(", ", connStrings);

        }
    }
}