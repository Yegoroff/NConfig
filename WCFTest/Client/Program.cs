using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using Services.ServiceContracts;
using NConfig;
using System.ServiceModel.Configuration;
using System.Reflection;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NConfigurator.UsingFile("ClientSvc.config").SetAsSystemDefault();

            Thread.Sleep(1000);
            Run("WsHttpSimple");
            Console.ReadLine();
        }

        static void Run(string endpointConfigurationName)
        {
            Console.WriteLine("Press enter to invoke PrintMessage operation on the service with " + endpointConfigurationName + " endpoint configuration");
            Console.ReadLine();

            ChannelFactory<ISimpleService> factory =  new ChannelFactory<ISimpleService>(endpointConfigurationName);
            ISimpleService proxy = factory.CreateChannel();

            proxy.PrintMessage("1");
            proxy.PrintMessage("2");
            proxy.PrintMessage("3");

            ((ICommunicationObject)proxy).Close();
        }
    }
}
