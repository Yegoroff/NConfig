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

            NConfig.NSystemConfigurator.CheckSubstituted();

            ClientSection cs = HttpContext.Current.GetSection("system.serviceModel/client") as ClientSection;
            int cc = cs.Endpoints.Count;

//            ISimpleService client = new SimpleServiceClient("WSHttpBinding_ISimpleService");

//            client.PrintMessage("Web Client message.");
        }
    }
}
