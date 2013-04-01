using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Web.Services;

namespace RentIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "http://rentit.itu.dk/RentIt27/WebServices/")]
    public interface IRentItService
    {
        #region Authentication

        [OperationContract]
        [WebGet(    UriTemplate = "/auth?user={username}&password={password}",
                    ResponseFormat = WebMessageFormat.Json)]
        string Authorize(string username, string password);

        #endregion

        #region Accounts

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts?types={types}&info={info}&include_banned={include_banned}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetAccounts(string types, string info, string include_banned);

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts/{user}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetAccount(string user);

        [OperationContract]
        [WebInvoke( Method = "PUT",
                    UriTemplate = "/accounts/{user}",
                    RequestFormat = WebMessageFormat.Json)]
        void UpdateAccount(string user, AccountData data);

        [OperationContract]
        [WebInvoke( Method = "POST",
                    UriTemplate = "/accounts/{user}",
                    RequestFormat = WebMessageFormat.Json)]
        void CreateAccount(string user, AccountData data);

        #endregion

        #region Products

        [OperationContract]
        [WebGet(    UriTemplate = "/products?search={search}&type={type}&info={info}&unpublished={unpublished}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetProducts(string search, string type, string info, string unpublished);

        [OperationContract]
        [WebGet(    UriTemplate = "/products/{id}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetProduct(uint id);

        [OperationContract]
        [WebInvoke( Method = "PUT",
                    UriTemplate = "/products/{id}",
                    RequestFormat = WebMessageFormat.Json)]
        void UpdateProduct(uint id, ProductData data);

        [OperationContract]
        [WebInvoke( Method = "POST",
                    UriTemplate = "/products/{id}")]
        void UpdateProductMedia(uint id, Stream media);

        [OperationContract]
        [WebInvoke( Method = "DELETE",
                    UriTemplate = "/products/{id}",
                    RequestFormat = WebMessageFormat.Json)]
        void DeleteProduct(uint id);

        [OperationContract]
        [WebGet(    UriTemplate = "/products/{id}/rating",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetProductRating(uint id);

        [OperationContract]
        [WebInvoke( Method = "PUT",
                    UriTemplate = "/products/{id}/rating",
                    RequestFormat = WebMessageFormat.Json)]
        void UpdateProductRating(uint id, RatingData data);

        #endregion

        #region Credits

        [OperationContract]
        [WebInvoke( Method = "POST",
                    UriTemplate = "/credits",
                    RequestFormat = WebMessageFormat.Json)]
        void BuyCredits(uint id, CreditsData data);

        #endregion

        #region Purchases

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts/{customer}/purchases?purchases={purchases}&info={info}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetPurchases(string customer, string purchases, string info);

        [OperationContract]
        [WebInvoke( Method = "POST",
                    UriTemplate = "/accounts/{customer}/purchases",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        string MakePurchases(string customer, PurchaseData data);

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts/{customer}/purchases/{id}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetPurchase(string customer, uint id);

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts/{customer}/purchases/{id}/media")]
        Stream GetPurchasedMedia(string customer, uint id);

        #endregion

        #region Content Provider Products

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts/{provider}/products?search={search}&type={type}&info={info}&unpublished={unpublished}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetProviderProducts(string provider, string search, string type, string info, string unpublished);

        [OperationContract]
        [WebInvoke( Method = "POST",
                    UriTemplate = "/accounts/{provider}/products",
                    RequestFormat = WebMessageFormat.Json)]
        string CreateProviderProduct(string provider, ProductData data);

        [OperationContract]
        [WebGet(    UriTemplate = "/accounts/{provider}/products/{id}",
                    ResponseFormat = WebMessageFormat.Json)]
        string GetProviderProduct(string provider, uint id);

        [OperationContract]
        [WebInvoke( Method = "PUT",
                    UriTemplate = "/accounts/{provider}/products/{id}",
                    RequestFormat = WebMessageFormat.Json)]
        void UpdateProviderProduct(string provider, uint id, ProductData data);

        [OperationContract]
        [WebInvoke( Method = "POST",
                    UriTemplate = "/accounts/{provider}/products/{id}")]
        void UpdateProviderProductMedia(string provider, uint id, Stream media);

        [OperationContract]
        [WebInvoke( Method = "DELETE",
                    UriTemplate = "/accounts/{provider}/products/{id}",
                    RequestFormat = WebMessageFormat.Json)]
        void DeleteProviderProduct(string provider, uint id);

        #endregion

    }

    [DataContract]
    public class TokenData
    {
        [DataMember]
        public string token { get; set; }
        [DataMember]
        public string expires { get; set; }
    }

    [DataContract]
    public class AccountData
    {
        [DataMember]
        public string user { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public bool? banned { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public AddressData address { get; set; }
        [DataMember]
        public string birth { get; set; }
        [DataMember]
        public string about { get; set; }
        [DataMember]
        public uint? credits { get; set; }
        [DataMember]
        public string created { get; set; }
        [DataMember]
        public string authenticated { get; set; }
    }

    [DataContract]
    public class AddressData
    {
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public uint? postal { get; set; }
        [DataMember]
        public string country { get; set; }
    }

    [DataContract]
    public class ProductData
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public PriceData price { get; set; }
        [DataMember]
        public RatingData rating { get; set; }
        [DataMember]
        public string owner { get; set; }
        [DataMember]
        public MetaData[] meta { get; set; }
        [DataMember]
        public bool? published { get; set; }
    }

    [DataContract]
    public class PriceData
    {
        [DataMember]
        public uint buy { get; set; }
        [DataMember]
        public uint? rent { get; set; }
    }

    [DataContract]
    public class RatingData
    {
        [DataMember]
        public int score { get; set; }
        [DataMember]
        public uint count { get; set; }
    }

    [DataContract]
    public class MetaData
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class CreditsData
    {
        [DataMember]
        public uint credits { get; set; }
    }

    [DataContract]
    public class PurchaseData
    {   
        [DataMember]
        public string purchased { get; set; }
        [DataMember]
        public uint paid { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string expires { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public uint product { get; set; }
    }

    [DataContract]
    public class IdData
    {
        [DataMember]
        public uint id { get; set; }
    }

}
