using System.ServiceModel;
using System.Xml.Linq;
using API.DTOs;

namespace API.Services
{
    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        string Test(string s);

        [OperationContract]
        void XmlMethod(System.Xml.Linq.XElement xml);

        // [OperationContract]
        // MyCustomModel TestCustomModel(MyCustomModel inputModel);

        [OperationContract]
        PlantSessionDto PlantLogin(String Key, String PlantId);
    }
}