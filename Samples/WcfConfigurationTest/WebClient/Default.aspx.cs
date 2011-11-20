using System;
using System.ServiceModel.Configuration;
using System.Threading;
using System.Web;
using WebClient.TestService;

namespace WebClient
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Give time to ServiceHost to initialize.
            Thread.Sleep(1500);

            // Read configuation section and print wcf service host url to page.
            var wcfClientSection = HttpContext.Current.GetSection("system.serviceModel/client") as ClientSection;
            if (wcfClientSection.Endpoints.Count > 0)
                endpointAddress.InnerText = wcfClientSection.Endpoints[0].Address.AbsoluteUri;


            // Send message to wcf service host.
            ISimpleService client = new SimpleServiceClient();
            client.PrintMessage("Calling from Web Client.");
        }
    }
}
