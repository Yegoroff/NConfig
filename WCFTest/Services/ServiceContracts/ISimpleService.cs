using System.ServiceModel;

namespace Services.ServiceContracts
{
    [ServiceContract]
    public interface ISimpleService
    {
        [OperationContract]
        string PrintMessage(string message);
    }
}
