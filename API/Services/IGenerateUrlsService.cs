using System.ServiceModel;
using API.DTOs.Product;

namespace API.Services
{
    [ServiceContract]
    public interface IGenerateUrlsService
    {
        [OperationContract]
        string Test(string s);

        [OperationContract]
        void XmlMethod(System.Xml.Linq.XElement xml);

        [OperationContract]
        List<String> GenerateShortUrls(String Token,int Quantity);

        [OperationContract]
        List<FetchAllProducts> FetchPlantProducts(String Token);
    }
}