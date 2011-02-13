using System;
using System.ServiceModel;
using NConfig;
using Services.Services;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            NConfigurator.UsingFile("HostSvc.config").SetAsSystemDefault();

            ServiceHost host = new ServiceHost(typeof(SimpleService));
            host.Open();

            Console.WriteLine("Host Started at " + host.ChannelDispatchers[0].Listener.Uri);
            Console.ReadLine();
            host.Close();
        }
    }
}
