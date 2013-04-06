using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using RentIt;
using RentIt.Services;

namespace Services.Controllers
{
    public class ProductController
    {

        //TODO: Make calls to ControlledProduct instead of Product
        
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private readonly CoreConverter _c;

        public ProductController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream GetProducts(string search, string types, string info, string unpublished)
        {
            try {

                var invoker = _h.Authorize();
                var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.accType : "";

                // PRODUCE RESPONSE

                var returnProducts = GetProductsHelper(null, search, types, info, unpublished);

                string[] keep = null;
                if (info == null || info.Equals("more")) {

                  keep = !accType.Equals("Admin")
                            ? new[] {"title", "description", "type", "price", "owner"}
                            : new[] {"title", "description", "type", "price", "owner", "published"};
                } else if (info.Equals("detailed")) {

                  keep = !accType.Equals("Admin")
                            ? new[] {"title", "description", "type", "price", "rating", "owner", "meta"}
                            : new[] {"title", "description", "type", "price", "rating", "owner", "meta", "published"};
                }

              _h.Success();

              if (info == null || info.Equals("detailed")) return _j.Json(_h.Map(returnProducts, p => _c.Convert(p)), keep);
              if (info.Equals("more")) return _j.Json(_h.Map(returnProducts, p => _c.Convert(p)), keep);
              if (info.Equals("id")) return _j.Json(_h.Map(returnProducts, p => (uint)p.id));  // Only ids are returned
              throw new BadRequestException(); // Never happens
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
        }
            
        public Stream GetProduct(string id)
        {
            try
            {
                // VERIFY

                int pId = (int)_h.Uint(id);

                // AUTHORIZE

                var invoker = _h.Authorize();
                var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.accType : "";
                var user = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.user : "";

                // RETRIEVE PRODUCT

                // NB: MUST THROW EXCEPTION IF WE ARE NOT ALLOWED TO VIEW PRODUCT, BECAUSE IT IS UNPUBLISHED!
                ProductData product = _c.Convert(Product.getProductById(pId));

                // PRODUCE RESPONSE

                // Normal users do not get the publish status of products
                if (!Ops.compareUsernames(user, product.owner) && !accType.Equals("Admin")) product.published = null;

                _h.Success();

                return _j.Json(product);
            }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (BadRequestException) { return _h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.NoSuchProduct) { return _h.Failure(404); }
            catch (Exception) { return _h.Failure(500); }
        }

        public void UpdateProduct(string id, ProductData data)
        {
            try
            {
                // VERIFY

                int pId = (int) _h.Uint(id);

                var invoker = _h.Authorize();

                // UPDATE DATA

                bool outdated = true;
                while (outdated)
                {
                    try
                    {
                        var product = Product.getProductById(pId);
                        var updated = _c.Merge(product, data);
                        Product.update(updated);

                        // If we get so far, the update went as planned, so we can quit the loop
                        outdated = false;
                    }
                    catch (ProductExceptions.OutdatedData) { /* Exception = load latest data and update based on it */ }
                }

                // SIGNAL SUCCESS

                _h.Success(204);
            }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (BadRequestException) { _h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (ProductExceptions.TooLargeData) { _h.Failure(413); }
            catch (Exception) { _h.Failure(500); }
        }

        public void UpdateProductMedia(string id, Stream data)
        {
            try
            {
                // VERIFY

                int pId;
                try { pId = (int) _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                string mime = _h.Header("Content-Type");
                if (string.IsNullOrEmpty(mime)) throw new BadRequestException();

                bool mimeOk = false;
                var product = Product.getProductById(pId);
                foreach (string m in Product.getMimesForProductType(product.productType)) {

                    if (m.Equals(mime)) { mimeOk = true; break; }
                }

                if (data == null || !mimeOk) throw new BadRequestException();

                var invoker = _h.Authorize();

                // PERSIST

                Product.persistMedia((uint) pId, mime, data);

                // SIGNAL SUCCESS

                _h.Success(204);
            }
            catch (BadRequestException) { _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); }
        }

        public void UpdateProductThumbnail(string id, Stream data)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                string mime = _h.Header("Content-Type");
                if (mime == null || !(mime.Equals("image/png") || mime.Equals("image/gif") || mime.Equals("image/jpeg"))) throw new BadRequestException();

                if (data == null) throw new BadRequestException();

                var invoker = _h.Authorize();

                // PERSIST

                Product.persistMediaThumbnail(pId, mime, data);

                // SIGNAL SUCCESS

                _h.Success(204);
            }
            catch (BadRequestException) { _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); }
        }

        public void DeleteProduct(string id)
        {
            _h.Failure(401);
            return;
        }

        public Stream GetProductRating(string id)
        {
            try
            {
                // AUTHORIZE

                var invoker = _h.Authorize();

                // RETRIEVE RATING

                int pId;
                try { pId = (int)_h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                RatingData rating = _c.Convert(_h.OrNull(Product.getProductById(pId).rating));

                // PRODUCE RESPONSE

                _h.Success();

                return _j.Json(rating);
            }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (NotFoundException) { return _h.Failure(404); }
            catch (ProductExceptions.ArgumentException) { return _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { return _h.Failure(404); }
            catch (Exception) { return _h.Failure(500); }
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            try
            {
                // VERIFY

                int pId;
                try { pId = (int)_h.Uint(id); }
                catch(BadRequestException){ throw new NotFoundException(); }

                var invoker = _h.Authorize();

                string user;
                if (invoker.IsAuth) user = ((PermissionsUtil.Invoker.Auth)invoker).Item.user;
                else throw new PermissionExceptions.PermissionDenied();

                // Rating is valid
                if(data == null || (data.score >= -5 && data.score <= 5)) throw new BadRequestException();

                // ADD RATING

                Product.rateProduct(pId, user, data.score);

                // SIGNAL SUCCESS

                _h.Success(204);
            }
            catch (BadRequestException) { _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (NotFoundException) { _h.Failure(404); }
            catch (ProductExceptions.ArgumentException) { _h.Failure(400); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (Exception) { _h.Failure(500); }
        }

        public Stream GetProductThumbnail(string id)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

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

        public Stream GetSupportedProductTypes()
        {
            try
            {
                var invoker = _h.Authorize();

                _h.Success();
                return _j.Json(Product.getListOfProductTypes(/*invoker*/));
            }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
        }

        /////////////////////////////////

        /// <summary>
        /// Retrieves a collection of products, either from the whole system, or only from a specific content provider
        /// </summary>
        /// <param name="target">target == null, then all products are used, else only products of the content provider with the name </param>
        /// <returns>IEnumerable of Products</returns>
        internal IEnumerable<ProductTypes.Product> GetProductsHelper(string target, string search, string types, string info, string unpublished)
        {
            // VALIDATE PARAMETERS

            search = _h.DefaultString(search, ""); // Default
            types = _h.DefaultString(types, ""); // Default
            HashSet<string> fullTypes = _h.ExpandProductTypes(types);

            info = _h.DefaultString(info, "id");
            info = _h.OneOf(info, "id", "more", "detailed");

            unpublished = _h.DefaultString(unpublished, "false");
            bool alsoUnpublished = _h.Boolean(unpublished);

            ProductTypes.PublishedStatus status = alsoUnpublished
                                                        ? ProductTypes.PublishedStatus.Both
                                                        : ProductTypes.PublishedStatus.Published;

            // AUTHORIZE

            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.accType : "";

            // RETRIEVE PRODUCTS
            IEnumerable<ProductTypes.Product> returnProducts;
            FSharpList<ProductTypes.Product> products = ListModule.Empty<ProductTypes.Product>();
            if (fullTypes.Count == 0)
            {
                if (string.IsNullOrEmpty(target)) products = Product.getAllProducts(status);
                else products = Product.getAllProductsByUser(target, status);
            }
            else
            {
                foreach (string type in fullTypes)
                {products = ListModule.Append(products, Product.getAllProductsByType(type, status)); }

                if (!string.IsNullOrEmpty(target)) {

                    products = ListModule.OfArray(_h.Map(products, p => p.owner.Equals(target) ? p : null));
                }
            }

            if (!search.Equals(""))
            {
                FSharpSet<ProductTypes.Product> searchProducts = SetModule.OfList(Product.searchProducts(search));
                returnProducts = SetModule.Intersect(SetModule.OfList(products), searchProducts);
            }
            else
            {
                returnProducts = products;
            }

            // PRODUCE RESPONSE

            return returnProducts;
        }
    }
}