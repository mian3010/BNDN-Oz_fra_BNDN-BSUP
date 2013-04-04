using RentIt;
using RentIt.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace Services.Controllers
{
    public class ProviderController
    {

        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private CoreConverter _c;

        public ProviderController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream DefaultGetProviderProducts(string provider)
        {
            return GetProviderProducts(provider, null, null, null, null);
        }

        public Stream GetProviderProducts(string provider, string search, string type, string info, string unpublished)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            OutgoingWebResponseContext response = _h.GetResponse();
        }

        public Stream CreateProviderProduct(string provider, ProductData data)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            OutgoingWebResponseContext response = _h.GetResponse();

            try
            {
                ProductTypes.Product tmpP = _h.ProductDataToProductType(data);
                ProductTypes.Product persistedP = Product.persist(tmpP);
                response.StatusCode = HttpStatusCode.NoContent;
                // Return the ID --> persistedP.id
            }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }

            return null;
        }

        public Stream GetProviderProduct(string provider, string id)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            OutgoingWebResponseContext response = _h.GetResponse();

        }

        public void UpdateProviderProduct(string provider, string id, ProductData data)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            OutgoingWebResponseContext response = _h.GetResponse();

        }

        public void UpdateProviderProductMedia(string provider, string id, Stream media)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();

        }

        public void DeleteProviderProduct(string provider, string id)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();

        }

        public Stream GetProviderProductThumbnail(string provider, string id)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            OutgoingWebResponseContext response = _h.GetResponse();

        }

        public void UpdateProviderProductThumbnail(string provider, string id, Stream media)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();

        }
    }
}