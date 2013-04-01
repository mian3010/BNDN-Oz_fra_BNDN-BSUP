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

namespace RentIt
{

    [ServiceBehavior(Namespace = "http://rentit.itu.dk/RentIt27/WebServices/")]
    public class RentItService : IRentItService
    {
        #region Setup

        private Helper h;
        private JsonSerializer j;
        private CoreConverter c;

        private AuthenticationController auth;
        private AccountController account;

        public RentItService() : this(new Helper()) { }

        public RentItService(Helper h) : this(h, new JsonSerializer(h), new CoreConverter(h)) { }

        public RentItService(Helper helper, JsonSerializer json, CoreConverter converter){

            h = helper;
            j = json;
            c = converter;

            auth = new AuthenticationController(h, j, c);
            account = new AccountController(h, j, c);
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
            return h.Failure(501);
        }

        public string GetProduct(uint id)
        {
            return h.Failure(501);
        }

        public void UpdateProduct(uint id, ProductData data)
        {
            h.Failure(501);
        }

        public void UpdateProductMedia(uint id, System.IO.Stream media)
        {
            h.Failure(501);
        }

        public void DeleteProduct(uint id)
        {
            h.Failure(501);
        }

        public string GetProductRating(uint id)
        {
            return h.Failure(501);
        }

        public void UpdateProductRating(uint id, RatingData data)
        {
            h.Failure(501);
        }

        #endregion

        #region Credits

        public void BuyCredits(uint id, CreditsData data)
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

        public string GetPurchase(string customer, uint id)
        {
            return h.Failure(501);
        }

        public System.IO.Stream GetPurchasedMedia(string customer, uint id)
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

        public string GetProviderProduct(string provider, uint id)
        {
            return h.Failure(501);
        }

        public void UpdateProviderProduct(string provider, uint id, ProductData data)
        {
            h.Failure(501);
        }

        public void UpdateProviderProductMedia(string provider, uint id, System.IO.Stream media)
        {
            h.Failure(501);
        }

        public void DeleteProviderProduct(string provider, uint id)
        {
            h.Failure(501);
        }

        #endregion

    }
}
