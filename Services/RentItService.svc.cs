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

        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private readonly CoreConverter _c;

        private readonly AuthenticationController _auth;
        private readonly AccountController _account;
        private readonly ProductController _product;
        private readonly CreditsController _credits;

        public RentItService() : this(new Helper()) { }

        public RentItService(Helper h) : this(h, new JsonSerializer(h), new CoreConverter(h)) { }

        public RentItService(Helper helper, JsonSerializer json, CoreConverter converter){

            _h = helper;
            _j = json;
            _c = converter;

            _auth = new AuthenticationController(_h, _j, _c);
            _account = new AccountController(_h, _j, _c);
            _product = new ProductController(_h, _j, _c);
            _credits = new CreditsController(_h, _j, _c);
        }

        #endregion

        #region Authentication

        public Stream DefaultAuthorize()
        {
            return Authorize(null, null);
        }

        public Stream Authorize(string username, string password)
        {
            return _auth.Authorize(username, password);
        }

        #endregion

        #region Accounts

        public Stream DefaultGetAccounts()
        {
            return GetAccounts(null, null, null);
        }

        public Stream GetAccounts(string types, string info, string includeBanned)
        {
            return _account.GetAccounts(types, info, includeBanned);
        }

        public Stream GetAccount(string user)
        {
            return _account.GetAccount(user);
        }

        public void UpdateAccount(string user, AccountData data)
        {
            _account.UpdateAccount(user, data);
        }

        public void CreateAccount(string user, AccountData data)
        {
            _account.CreateAccount(user, data);
        }

        public Stream GetAcceptedCountries() 
        {
            return _account.GetAcceptedCountries();
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
            return _product.GetProducts(search, type, info, unpublished);
        }

        public Stream GetProduct(string id)
        {
            //return h.Failure(501);
            return _product.GetProduct(id);
        }

        public void UpdateProduct(string id, ProductData data)
        {
            //h.Failure(501);
            _product.UpdateProduct(id, data);
        }

        public void UpdateProductMedia(string id, Stream media)
        {
            //h.Failure(501);
            _product.UpdateProductMedia(id, media);
        }

        public void DeleteProduct(string id)
        {
            //h.Failure(501);
            _product.DeleteProduct(id);
        }

        public Stream GetProductRating(string id)
        {
            //return h.Failure(501);
            return _product.GetProductRating(id);
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            //h.Failure(501);
            _product.UpdateProductRating(id, data);
        }

        public Stream GetProductThumbnail(string id)
        {
            //return h.Failure(501);
            return _product.GetProductThumbnail(id);
        }

        public void UpdateProductThumbnail(string id, Stream media)
        {
            //h.Failure(501);
            _product.UpdateProductThumbnail(id, media);
        }

        public Stream GetSupportedProductTypes() 
        {
            return _product.GetSupportedProductTypes();
        }

        #endregion

        #region Credits

        public void BuyCredits(CreditsData data)
        {
            _credits.BuyCredits(data);
        }

        #endregion

        #region Purchases

        public Stream DefaultGetPurchases(string customer)
        {
            return GetPurchases(customer, null, null);
        }

        public Stream GetPurchases(string customer, string purchases, string info)
        {
            return _h.Failure(501);
        }

        public Stream MakePurchases(string customer, PurchaseData[] data)
        {
            return _h.Failure(501);
        }

        public Stream GetPurchase(string customer, string id)
        {
            return _h.Failure(501);
        }

        public Stream GetPurchasedMedia(string customer, string id)
        {
            _h.Failure(501);
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
            return _h.Failure(501);
        }

        public Stream CreateProviderProduct(string provider, ProductData data)
        {
            return _h.Failure(501);
        }

        public Stream GetProviderProduct(string provider, string id)
        {
            return _h.Failure(501);
        }

        public void UpdateProviderProduct(string provider, string id, ProductData data)
        {
            _h.Failure(501);
        }

        public void UpdateProviderProductMedia(string provider, string id, Stream media)
        {
            _h.Failure(501);
        }

        public void DeleteProviderProduct(string provider, string id)
        {
            _h.Failure(501);
        }

        public Stream GetProviderProductThumbnail(string provider, string id)
        {
            _h.Failure(501);
            return null;
        }

        public void UpdateProviderProductThumbnail(string provider, string id, Stream media)
        {
            _h.Failure(501);
        }

        #endregion

        }
}
