using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace WebClient.Nest
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var appSettingsExpected = new List<string> { "extendedParam", "rootParam", "nestedParam" };
            var connStringsExpected = new List<string> { "LocalSqlServer", "ExtendedConnection", "ApplicationServices", "RootConnection", "NestedConnection" };

            var appSettings = new List<string>();
            foreach (var item in ConfigurationManager.AppSettings)
            {
                appSettings.Add(item.ToString());
            }

            Debug.Assert(appSettings.SequenceEqual(appSettingsExpected), "Invalid Application Settings");

            var connStrings = new List<string>();
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                connStrings.Add(item.Name);
            }

            Debug.Assert(connStrings.SequenceEqual(connStringsExpected), "Invalid Connection Strings");

            applicationSettings.InnerText = string.Join(", ", appSettings);
            connectionStrings.InnerText = string.Join(", ", connStrings);

        }
    }
}