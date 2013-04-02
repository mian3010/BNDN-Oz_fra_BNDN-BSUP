using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.Reflection;
using RentIt;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using System.Web.Services;
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

        public RentItService() : this(new Helper()) { }

        public RentItService(Helper h) : this(h, new JsonSerializer(h), new CoreConverter(h)) { }

        public RentItService(Helper helper, JsonSerializer json, CoreConverter converter){

            h = helper;
            j = json;
            c = converter;

            auth = new AuthenticationController(h, j, c);
            account = new AccountController(h, j, c);
            product = new ProductController(h, j, c);
        }

        #endregion

        #region Authentication

        public string Authorize(string username, string password)
        {
            return auth.Authorize(username, password);
        }

        #endregion

        #region Accounts

        public string GetAccounts(string types, string info, string include_banned)
        {
            return account.GetAccounts(types, info, include_banned);
        }

        public string GetAccount(string user)
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

        #endregion

        #region Products

        public string GetProducts(string search, string type, string info, string unpublished)
        {
            return product.GetProducts(search, type, info, unpublished);
        }

        public string GetProduct(string id)
        {
            return product.GetProduct(id);
        }

        public void UpdateProduct(string id, ProductData data)
        {
            product.UpdateProduct(id, data);
        }

        public void UpdateProductMedia(string id, Stream media)
        {
            product.UpdateProductMedia(id, media);
        }

        public void DeleteProduct(string id)
        {
            product.DeleteProduct(id);
        }

        public string GetProductRating(string id)
        {
            return product.GetProductRating(id);
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            product.UpdateProductRating(id, data);
        }

        public Stream GetProductThumbnail(string id)
        {
            return product.GetProductThumbnail(id);
        }

        public void UpdateProductThumbnail(string id, Stream media)
        {
            product.UpdateProductThumbnail(id, media);
        }

        #endregion

        #region Credits

        public void BuyCredits(CreditsData data)
        {
            h.Failure(501);
        }

        #endregion

        #region Purchases

        public string GetPurchases(string customer, string purchases, string info)
        {
            return h.Failure(501);
        }

        public string MakePurchases(string customer, PurchaseData data)
        {
            return h.Failure(501);
        }

        public string GetPurchase(string customer, string id)
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

        public string GetProviderProducts(string provider, string search, string type, string info, string unpublished)
        {
            return h.Failure(501);
        }

        public string CreateProviderProduct(string provider, ProductData data)
        {
            return h.Failure(501);
        }

        public string GetProviderProduct(string provider, string id)
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
