using System.ServiceModel;
using RentIt.Services;
using RentIt.Services.Controllers;
using System.IO;
using Services.Controllers;

namespace RentIt
{

    [ServiceBehavior(Namespace = "http://rentit.itu.dk/RentIt27/WebServices/")]
    public class RentItService : IRentItService
    {
        #region Setup

        private readonly Helper h;
        private readonly JsonSerializer j;
        private readonly CoreConverter c;

        private readonly AuthenticationController auth;
        private readonly AccountController account;
        private readonly ProductController product;
        private readonly CreditsController credits;

        public RentItService() : this(new Helper()) { }

        public RentItService(Helper h) : this(h, new JsonSerializer(h), new CoreConverter(h)) { }

        public RentItService(Helper helper, JsonSerializer json, CoreConverter converter){

            h = helper;
            j = json;
            c = converter;

            auth = new AuthenticationController(h, j, c);
            account = new AccountController(h, j, c);
            product = new ProductController(h, j, c);
            credits = new CreditsController(h, j, c);
        }

        #endregion

        #region Authentication

        public Stream DefaultAuthorize()
        {
            return Authorize(null, null);
        }

        public Stream Authorize(string username, string password)
        {
            return auth.Authorize(username, password);
        }

        #endregion

        #region Accounts

        public Stream DefaultGetAccounts()
        {
            return GetAccounts(null, null, null);
        }

        public Stream GetAccounts(string types, string info, string include_banned)
        {
            return account.GetAccounts(types, info, include_banned);
        }

        public Stream GetAccount(string user)
        {
            return account.GetAccount(user);
        }

        public void UpdateAccount(string user, AccountData data)
        {
            account.UpdateAccount(user, data);
        }

        public void CreateAccount(string user, AccountData data)
        {
            account.CreateAccount(user, data);
        }

        public Stream GetAcceptedCountries() 
        {
            return account.GetAcceptedCountries();
        }

        #endregion

        #region Products

        public Stream DefaultGetProducts()
        {
            return GetProducts(null, null, null, null);
        }

        public Stream GetProducts(string search, string type, string info, string unpublished)
        {
            //return h.Failure(501);
            return product.GetProducts(search, type, info, unpublished);
        }

        public Stream GetProduct(string id)
        {
            //return h.Failure(501);
            return product.GetProduct(id);
        }

        public void UpdateProduct(string id, ProductData data)
        {
            //h.Failure(501);
            product.UpdateProduct(id, data);
        }

        public void UpdateProductMedia(string id, Stream media)
        {
            //h.Failure(501);
            product.UpdateProductMedia(id, media);
        }

        public void DeleteProduct(string id)
        {
            //h.Failure(501);
            product.DeleteProduct(id);
        }

        public Stream GetProductRating(string id)
        {
            //return h.Failure(501);
            return product.GetProductRating(id);
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            //h.Failure(501);
            product.UpdateProductRating(id, data);
        }

        public Stream GetProductThumbnail(string id)
        {
            //return h.Failure(501);
            return product.GetProductThumbnail(id);
        }

        public void UpdateProductThumbnail(string id, Stream media)
        {
            //h.Failure(501);
            product.UpdateProductThumbnail(id, media);
        }

        public Stream GetSupportedProductTypes() 
        {
            return product.GetSupportedProductTypes();
        }

        #endregion

        #region Credits

        public void BuyCredits(CreditsData data)
        {
            credits.BuyCredits(data);
        }

        #endregion

        #region Purchases

        public Stream DefaultGetPurchases(string customer)
        {
            return GetPurchases(customer, null, null);
        }

        public Stream GetPurchases(string customer, string purchases, string info)
        {
            return h.Failure(501);
        }

        public Stream MakePurchases(string customer, PurchaseData data)
        {
            return h.Failure(501);
        }

        public Stream GetPurchase(string customer, string id)
        {
            return h.Failure(501);
        }

        public Stream GetPurchasedMedia(string customer, string id)
        {
            h.Failure(501);
            return null;
        }

        #endregion

        #region Provider Products

        public Stream DefaultGetProviderProducts(string provider)
        {
            return GetProviderProducts(provider, null, null, null, null);
        }

        public Stream GetProviderProducts(string provider, string search, string type, string info, string unpublished)
        {
            return h.Failure(501);
        }

        public Stream CreateProviderProduct(string provider, ProductData data)
        {
            return h.Failure(501);
        }

        public Stream GetProviderProduct(string provider, string id)
        {
            return h.Failure(501);
        }

        public void UpdateProviderProduct(string provider, string id, ProductData data)
        {
            h.Failure(501);
        }

        public void UpdateProviderProductMedia(string provider, string id, Stream media)
        {
            h.Failure(501);
        }

        public void DeleteProviderProduct(string provider, string id)
        {
            h.Failure(501);
        }

        public Stream GetProviderProductThumbnail(string provider, string id)
        {
            h.Failure(501);
            return null;
        }

        public void UpdateProviderProductThumbnail(string provider, string id, Stream media)
        {
            h.Failure(501);
        }

        #endregion

        }
}
