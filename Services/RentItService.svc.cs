using System.ServiceModel;
using System.ServiceModel.Web;
using RentIt.Services;
using RentIt.Services.Controllers;
using System.IO;
using Services;
using Services.Controllers;

namespace RentIt {

  [ServiceBehavior(Namespace = "http://rentit.itu.dk/RentIt27/WebServices/")]
  public class RentItService : IRentItService {
    #region Setup

    private readonly Helper _h;
    private readonly JsonSerializer _j;
    private readonly CoreConverter _c;

    private readonly AuthenticationController _auth;
    private readonly AccountController _account;
    private readonly ProductController _product;
    private readonly ProviderController _provider;
    private readonly CreditsController _credits;

    public RentItService() : this(new Helper()) { }

    public RentItService(Helper h) : this(h, new JsonSerializer(h), new CoreConverter(h)) { }

    public RentItService(Helper helper, JsonSerializer json, CoreConverter converter) {

      _h = helper;
      _j = json;
      _c = converter;

      _auth = new AuthenticationController(_h, _j, _c);
      _account = new AccountController(_h, _j, _c);
      _product = new ProductController(_h, _j, _c);
      _provider = new ProviderController(_h, _j, _c, _product);
      _credits = new CreditsController(_h, _j, _c);
    }

    #endregion

    #region Authentication

    public Stream DefaultAuthorize() {
      return Authorize(null, null);
    }

    public Stream Authorize(string username, string password) {
      return _auth.Authorize(username, password);
    }

    #endregion

    #region Accounts
      
    public Stream GetAccounts() {
      return _account.GetAccounts(QueryParameters.get("types"), QueryParameters.get("info"), QueryParameters.get("includeBanned"));
    }

    public Stream GetAccount(string user) {
      return _account.GetAccount(user);
    }

    public void UpdateAccount(string user, AccountData data) {
      _account.UpdateAccount(user, data);
    }

    public void CreateAccount(string user, AccountData data) {
      _account.CreateAccount(user, data);
    }

    public Stream GetAcceptedCountries() {
      return _account.GetAcceptedCountries();
    }

    #endregion

    #region Products

    public Stream GetProducts() {
      return _product.GetProducts(QueryParameters.get("search"), QueryParameters.get("type"), QueryParameters.get("info"), QueryParameters.get("unpublished"));
    }

    public Stream GetProduct(string id) {
      return _product.GetProduct(id);
    }

    public void UpdateProduct(string id, ProductData data) {
      _product.UpdateProduct(id, data);
    }

    public void UpdateProductMedia(string id, Stream media) {
      _product.UpdateProductMedia(id, media);
    }

    public void DeleteProduct(string id) {
      _product.DeleteProduct(id);
    }

    public Stream GetProductRating(string id) {
      return _product.GetProductRating(id);
    }

    public void UpdateProductRating(string id, RatingData data) {
      _product.UpdateProductRating(id, data);
    }

    public Stream GetProductThumbnail(string id) {
      return _product.GetProductThumbnail(id);
    }

    public void UpdateProductThumbnail(string id, Stream media) {
      _product.UpdateProductThumbnail(id, media);
    }

    public Stream GetSupportedProductTypes() {
      return _product.GetSupportedProductTypes();
    }

    #endregion

    #region Credits

    public void BuyCredits(CreditsData data) {
      _credits.BuyCredits(data);
    }

    #endregion

    #region Purchases

    public Stream DefaultGetPurchases(string customer) {
      return GetPurchases(customer, null, null);
    }

    public Stream GetPurchases(string customer, string purchases, string info) {
      return _credits.GetPurchases(customer, purchases, info);
    }

    public Stream MakePurchases(string customer, PurchaseData[] data) {
      return _credits.MakePurchases(customer, data);
    }

    public Stream GetPurchase(string customer, string id) {
      return _credits.GetPurchase(customer, id);
    }

    public Stream GetPurchasedMedia(string customer, string id) {
      return _credits.GetPurchasedMedia(customer, id);
    }

    #endregion

    #region Provider Products

    public Stream DefaultGetProviderProducts(string provider) {
      return GetProviderProducts(provider, null, null, null, null);
    }

    public Stream GetProviderProducts(string provider, string search, string type, string info, string unpublished) {
      return _provider.GetProviderProducts(provider, search, type, info, unpublished);
    }

    public Stream CreateProviderProduct(string provider, ProductData data) {
      return _provider.CreateProviderProduct(provider, data);
    }

    public Stream GetProviderProduct(string provider, string id) {
      return _provider.GetProviderProduct(provider, id);
    }

    public void UpdateProviderProduct(string provider, string id, ProductData data) {
      _provider.UpdateProviderProduct(provider, id, data);
    }

    public void UpdateProviderProductMedia(string provider, string id, Stream data) {
      _provider.UpdateProviderProductMedia(provider, id, data);
    }

    public void DeleteProviderProduct(string provider, string id) {
      _provider.DeleteProviderProduct(provider, id);
    }

    public Stream GetProviderProductThumbnail(string provider, string id) {
      return _provider.GetProviderProductThumbnail(provider, id);
    }

    public void UpdateProviderProductThumbnail(string provider, string id, Stream data) {
      _provider.UpdateProviderProductThumbnail(provider, id, data);
    }

    #endregion

  }
}
