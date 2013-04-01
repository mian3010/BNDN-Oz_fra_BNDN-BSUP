using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Services;

namespace RentIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "http://rentit.itu.dk/RentIt27/WebServices/")]
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
            UriTemplate = "/accounts/{user}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void UpdateAccount(string user, AccountData data);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/accounts/{user}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void CreateAccount(string user, AccountData data);

        #endregion

        #region Products

        [OperationContract]
        [WebGet(UriTemplate = "/products?search={search}&type={type}&info=id&unpublished={unpublished}",
        ResponseFormat = WebMessageFormat.Json)]
        uint[] GetProducts(string search, string type, bool unpublished);

        [OperationContract]
        [WebGet(UriTemplate = "/products?search={search}&type={type}&info={info}&unpublished={unpublished}",
        ResponseFormat = WebMessageFormat.Json)]
        ProductData[] GetProducts(string search, string type, string info, bool unpublished);

        [OperationContract]
        [WebGet(UriTemplate = "/products/{id}",
        ResponseFormat = WebMessageFormat.Json)]
        ProductData GetProduct(uint id);

        [OperationContract]
        [WebInvoke(Method = "PUT",
        UriTemplate = "/products/{id}",
        RequestFormat = WebMessageFormat.Json)]
        void UpdateProduct(uint id, ProductData data);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/products/{id}")]
        void UpdateProductMedia(uint id, System.IO.Stream media);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
        UriTemplate = "/products/{id}",
        RequestFormat = WebMessageFormat.Json)]
        void DeleteProduct(uint id);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/products/{id}/thumbnail")]
        void UpdateProductThumbnail(uint id, System.IO.Stream media);

        [OperationContract]
        [WebGet(UriTemplate = "/products/{id}/rating",
        ResponseFormat = WebMessageFormat.Json)]
        RatingData GetProductRating(uint id);

        [OperationContract]
        [WebInvoke(Method = "PUT",
        UriTemplate = "/products/{id}/rating",
        RequestFormat = WebMessageFormat.Json)]
        void UpdateProductRating(uint id, RatingData data);

        #endregion

        #region Purchases

        [OperationContract]
        [WebGet(UriTemplate = "/accounts/{customer}/purchases/{id}/media")]
        System.IO.Stream GetPurchasedMedia(string customer, uint id);

        [OperationContract]
        [WebGet(UriTemplate = "/accounts/{customer}/purchases/{id}/media/thumbnail")]
        System.IO.Stream GetPurchasedMedia(string customer, uint id);

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
