using System.ServiceModel;

namespace SoapWebApp.Interfaces
{
    [ServiceContract]
    public interface IFibonacciService
    {
        [OperationContract]
        Task<int> CalculateFibonacciAsync(int n);
    }
}
