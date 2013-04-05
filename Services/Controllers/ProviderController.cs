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
            try
            {
                var invoker = _h.Authorize();
                var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.accType : "";

                // PRODUCE RESPONSE

                var returnProducts = _p.GetProductsHelper(provider, search, type, info, unpublished);

                string[] keep = null;
                if (info.Equals("more"))
                {

                    keep = !accType.Equals("Admin") ? new[] { "title", "description", "type", "price" }
                                                    : new[] { "title", "description", "type", "price", "published" };
                }
                else if (info.Equals("detailed"))
                {

                    keep = !accType.Equals("Admin") ? new[] { "title", "description", "type", "price", "rating", "meta" }
                                                    : new[] { "title", "description", "type", "price", "rating", "meta", "published" };
                }

                _h.Success();

                if (info.Equals("detailed")) return _j.Json(_h.Map(returnProducts, p => _c.Convert(p)), keep);
                if (info.Equals("more")) return _j.Json(_h.Map(returnProducts, p => _c.Convert(p)), keep);
                if (info.Equals("id")) return _j.Json(_h.Map(returnProducts, p => (uint)p.id));  // Only ids are returned
                throw new BadRequestException(); // Never happens
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
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
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                var product = Product.getProductById((int)pId);
                if (!Ops.compareUsernames(product.owner, provider)) throw new NotFoundException();

                // EXECUTE

                return _p.GetProduct(id);
            }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (NotFoundException) { return _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { return _h.Failure(404); }
            catch (Exception) { return _h.Failure(500); }
        }

        public void UpdateProviderProduct(string provider, string id, ProductData data)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                var product = Product.getProductById((int)pId);
                if (!Ops.compareUsernames(product.owner, provider)) throw new NotFoundException();

                // EXECUTE

                _p.UpdateProduct(id, data);
            }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); } 
        }

        public void UpdateProviderProductMedia(string provider, string id, Stream data)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                var product = Product.getProductById((int)pId);
                if (!Ops.compareUsernames(product.owner, provider)) throw new NotFoundException();

                // EXECUTE

                _p.UpdateProductMedia(id, data);
            }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); } 
        }

        public void DeleteProviderProduct(string provider, string id)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                var product = Product.getProductById((int)pId);
                if (!Ops.compareUsernames(product.owner, provider)) throw new NotFoundException();

                // EXECUTE

                _p.DeleteProduct(id);
            }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); } 
        }

        public Stream GetProviderProductThumbnail(string provider, string id)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                var product = Product.getProductById((int) pId);
                if (!Ops.compareUsernames(product.owner, provider)) throw new NotFoundException();

                // LOAD

                var result = Product.getMediaThumbnail(pId);

                // SIGNAL SUCCESS

                _h.SetHeader("Content-Length", result.Item1.Length.ToString());
                _h.Success(200, result.Item2);

                return result.Item1;
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (NotFoundException) { return _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { return _h.Failure(404); }
            catch (ProductExceptions.NoSuchMedia) { return _h.Failure(404); }
            catch (Exception) { return _h.Failure(500); } 
        }

        public void UpdateProviderProductThumbnail(string provider, string id, Stream data)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                var product = Product.getProductById((int)pId);
                if (!Ops.compareUsernames(product.owner, provider)) throw new NotFoundException();

                // EXECUTE

                _p.UpdateProductThumbnail(id, data);
            }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); } 
        }

    }
}