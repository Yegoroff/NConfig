using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Services.Services;
using System.Security.Cryptography.X509Certificates;
using NConfig;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {

            //NConfigurator.FromFile("HostSvc.config").PromoteToSystemDefault();

            ServiceHost host = new ServiceHost(typeof(SimpleService));
            host.Open();

            Console.WriteLine("Host Started at " + host.ChannelDispatchers[0].Listener.Uri);
            Console.ReadLine();
            host.Close();
        }
    }
}
