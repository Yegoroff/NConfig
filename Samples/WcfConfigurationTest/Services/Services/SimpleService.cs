using System;
using Services.ServiceContracts;

namespace Services.Services
{
    public class SimpleService : ISimpleService
    {
        public string PrintMessage(string msg)
        {
            msg = "PrintMessage(" + msg + ")";
            Console.WriteLine(msg);
            return msg;
        }
    }
}
