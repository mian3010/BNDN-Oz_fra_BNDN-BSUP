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
        [WebGet(UriTemplate = "/auth?user={USERNAME}&password={PASSWORD}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string Authorise(string USERNAME, string PASSWORD);

        #endregion

        #region Accounts

        [OperationContract]
        [WebGet(UriTemplate = "/accounts?types={types}&info={info}&include_banned={include_banned}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetAccounts(string types, string info, bool include_banned);

        [OperationContract]
        [WebGet(UriTemplate = "/accounts/{user}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetAccount(string user);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "/acounts/{user}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string UpdateAccount(string user, AccountData data);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/accounts/{user}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string CreateAccount(string user, AccountData data);

        #endregion

        #region Products

        #endregion

    }

    [DataContract]
    public class AccountData
    {
        [DataMember]
        public string user { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string dateOfBirth { get; set; }
        [DataMember]
        public bool banned { get; set; }
        [DataMember]
        public string aboutMe { get; set; }
        [DataMember]
        public int balance { get; set; }
        [DataMember]
        public string accountType { get; set; }
        [DataMember]
        public int zipcode { get; set; }
        [DataMember]
        public string countryName { get; set; }
    }

}
