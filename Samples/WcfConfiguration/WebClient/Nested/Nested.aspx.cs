using System;
using System.Configuration;
using NConfig;

namespace WebClient.Nest
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var testSection = NConfigurator.Default.GetSection<TestConfigSection>();

            var configManagerTestSection = ConfigurationManager.GetSection("TestConfigSection") as TestConfigSection;

            var namedTestSection = NConfigurator.UsingFile(@"Config\Custom.config").GetSection<TestConfigSection>("NamedSection");

            NConfigDefault.InnerText = testSection.TestValue;
            ConfigManager.InnerText = configManagerTestSection.TestValue;
            NConfigNamed.InnerText = namedTestSection.TestValue;


            var appSettings =  "";
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                appSettings += "<p>" + key + " : " + ConfigurationManager.AppSettings[key] + "</p>";
            }
            AppSettings.InnerHtml = appSettings;

            var connectionStrings = "";
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                connectionStrings += "<p>" + connectionString.Name + " : " + connectionString.ConnectionString + "</p>";
            }
            ConnectionStrings.InnerHtml = connectionStrings;

        }
    }
}