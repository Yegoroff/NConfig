using System;
using System.ServiceModel;
using System.Threading;
using NConfig;
using Services.ServiceContracts;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NConfigurator.UsingFile("ClientSvc.config").SetAsSystemDefault();

            Thread.Sleep(1000); // this will get WCF service app some fora to run.
            Run("WsHttpSimple");

            Console.ReadLine();
        }

        static void Run(string endpointConfigurationName)
        {
            Console.WriteLine("Press enter to invoke PrintMessage operation on the service with " + endpointConfigurationName + " endpoint configuration");
            Console.ReadLine();

            var factory =  new ChannelFactory<ISimpleService>(endpointConfigurationName);
            ISimpleService proxy = factory.CreateChannel();

            proxy.PrintMessage("Calling form Console Client.");

            ((ICommunicationObject)proxy).Close();
        }
    }
}
