using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebClient.TestService;
using System.Threading;
using System.ServiceModel.Configuration;
using System.Reflection;
using System.Configuration;

namespace WebClient
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.Sleep(1500);


            ClientSection cs = HttpContext.Current.GetSection("system.serviceModel/client") as ClientSection;
            if (cs.Endpoints.Count > 0)
                endpointAddress.InnerText = cs.Endpoints[0].Address.AbsoluteUri;


            ISimpleService client = new SimpleServiceClient();
            client.PrintMessage("Calling from Web Client.");
        }
    }
}
