using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RentIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRentItService
    {
        #region Authentication

        [OperationContract]
        [WebGet(UriTemplate="/auth",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string Authorise(string USERNAME, string PASSWORD);

        #endregion

        #region Accounts

        [OperationContract]
        [WebGet(UriTemplate = "/accounts",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        [WebGet(UriTemplate = "/acounts/{USERNAME}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetData(int value);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "/acounts/{USERNAME}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetData(int value);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/accounts/{USERNAME}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetData(int value);

        #endregion
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
