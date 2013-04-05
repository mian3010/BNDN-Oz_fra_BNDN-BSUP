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

        // TODO: Change Product to ControlledProduct

        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private readonly CoreConverter _c;
        private readonly ProductController _p;

        public ProviderController(Helper helper, JsonSerializer json, CoreConverter converter, ProductController productController)
        {
            _h = helper;
            _j = json;
            _c = converter;
            _p = productController;
        }

        public Stream GetProviderProducts(string provider, string search, string type, string info, string unpublished)
        {
            return _h.Failure(501);
        }

        public Stream CreateProviderProduct(string provider, ProductData data)
        {
            try
            {
                // VERIFY

                if (data == null || string.IsNullOrEmpty(data.title)) throw new BadRequestException();
                _h.OneOf(data.type, Product.getListOfProductTypes());

                var invoker = _h.Authorize();

                // CREATE ACCOUNT

                data.published = false;
                data.owner = provider;

                ProductTypes.Product product = _c.Convert(data);

                var id = (uint) Product.persist(product).id;

                // SIGNAL SUCCESS

                _h.SetHeader("Location", "/accounts/" + provider + "/products/" + id);
                _h.Success(201);

                return _j.Json(_c.Convert(id));
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (ProductExceptions.TooLargeData) { return _h.Failure(413); }
            catch (Exception) { return _h.Failure(500); }
        }

        public Stream GetProviderProduct(string provider, string id)
        {
            return _h.Failure(501);
        }

        public void UpdateProviderProduct(string provider, string id, ProductData data)
        {
            _h.Failure(501);
        }

        public void UpdateProviderProductMedia(string provider, string id, Stream data)
        {
            _h.Failure(501);
        }

        public void DeleteProviderProduct(string provider, string id)
        {
            _h.Failure(501);
        }

        public Stream GetProviderProductThumbnail(string provider, string id)
        {
            return _h.Failure(501);
        }

        public void UpdateProviderProductThumbnail(string provider, string id, Stream data)
        {
            _h.Failure(501);
        }

    }
}