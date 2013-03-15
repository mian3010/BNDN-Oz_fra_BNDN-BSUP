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
        string GetAccounts(string types, string info, bool include_banned);

        [OperationContract]
        [WebGet(UriTemplate = "/acounts/{USERNAME}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetAccount(string USERNAME);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "/acounts/{USERNAME}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string UpdateAccount(string USERNAME, string email, string name, string address, string birth, string about, string password);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/accounts/{USERNAME}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string CreateAccount(string USERNAME, string email, string name, string address, string birth, string about, string password, string type);

        #endregion

        #region Products

        #endregion
    }
}
