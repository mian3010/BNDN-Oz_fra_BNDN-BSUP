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

        //TODO: Make calls to ControlledProduct instead og Product
        //TODO: Check for permissions to view unpublished products
        //TODO: Implement product search
        
        private readonly Helper h;
        private readonly JsonSerializer j;
        private CoreConverter c;

        public ProductController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            h = helper;
            j = json;
            c = converter;
        }

        public Stream GetProducts(string search, string types, string info, string unpublished)
        {
            try
            {
                // VALIDATE PARAMETERS

                search = h.DefaultString(search, ""); // Default
                types = h.DefaultString(types, ""); // Default
                HashSet<string> fullTypes = h.ExpandProductTypes(types);

                info = h.DefaultString(info, "id");
                info = h.OneOf(info, "id", "more", "detailed");

                unpublished = h.DefaultString(unpublished, "false");
                bool also_unpublished = h.Boolean(unpublished);

                // AUTHORIZE

                var invoker = h.Authorize();
                var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.accType : "";
            
                // RETRIEVE PRODUCTS

                FSharpList<ProductTypes.Product> products = ListModule.Empty<ProductTypes.Product>();
                
                if(fullTypes.Count == 0) products = Product.getAll();
                else foreach (string type in fullTypes)
                {
                    products = ListModule.Append(products, Product.getAllByType(type));
                }

                // Remove unpublished products
                if (!(also_unpublished /* && HAS_PERMISSION_TO_GET_UNPUBLISHED */)) products = Product.filterUnpublished(products);

                // PRODUCE RESPONSE

                string[] keep = null;
                if (info.Equals("more")) {

                    if (!accType.Equals("Admin")) keep = new string[]{ "title", "description", "type", "price", "owner" };
                    else keep =                          new string[]{ "title", "description", "type", "price", "owner", "published" };
                }
                else if (info.Equals("detailed")) {

                    if (!accType.Equals("Admin")) keep = new string[]{ "title", "description", "type", "price", "rating", "owner", "meta" };
                    else keep =                          new string[]{ "title", "description", "type", "price", "rating", "owner", "meta", "published" };
                }

                h.Success();

                if (info.Equals("detailed")) return j.Json(h.Map(products, p => c.Convert(p)), keep);
                if (info.Equals("more")) return j.Json(h.Map(products, p => c.Convert(p)), keep);
                if (info.Equals("id")) return j.Json(h.Map(products, p => (uint)p.id));  // Only ids are returned
                else throw new BadRequestException(); // Never happens
            }
            catch (BadRequestException) { return h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return h.Failure(403); }
            catch (Exception) { return h.Failure(500); }
        }
            
        public Stream GetProduct(string id)
        {
            try
            {
                // VERIFY

                int pId = (int)h.Uint(id);

                // AUTHORIZE

                var invoker = h.Authorize();
                var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.accType : "";
                var user = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth)invoker).Item.user : "";

                // RETRIEVE PRODUCT

                // NB: MUST THROW EXCEPTION IF WE ARE NOT ALLOWED TO VIEW PRODUCT, BECAUSE IT IS UNPUBLISHED!
                ProductData product = c.Convert(Product.getProductById(pId));

                // PRODUCE RESPONSE

                // Normal users do not get the publish status of products
                if (!Ops.compareUsernames(user, product.owner) && !accType.Equals("Admin")) product.published = null;

                h.Success();

                return j.Json(product);
            }
            catch (PermissionExceptions.PermissionDenied) { return h.Failure(403); }
            catch (BadRequestException) { return h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.NoSuchProduct) { return h.Failure(404); }
            catch (Exception) { return h.Failure(500); }
        }

        public void UpdateProduct(string id, ProductData data)
        {
            try
            {
                // VERIFY

                int pId = (int) h.Uint(id);

                var invoker = h.Authorize();

                // UPDATE DATA

                bool outdated = true;
                while (outdated)
                {
                    try
                    {
                        var product = Product.getProductById(pId);
                        var updated = c.Merge(product, data);
                        Product.update(updated);

                        // If we get so far, the update went as planned, so we can quit the loop
                        outdated = false;
                    }
                    catch (ProductExceptions.OutdatedData) { /* Exception = load latest data and update based on it */ }
                }

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (PermissionExceptions.PermissionDenied) { h.Failure(403); }
            catch (BadRequestException) { h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.NoSuchProduct) { h.Failure(404); }
            catch (ProductExceptions.TooLargeData) { h.Failure(413); }
            catch (Exception) { h.Failure(500); }
        }

        public void UpdateProductMedia(string id, Stream data)
        {
            try
            {
                // VERIFY

                int pId;
                try { pId = (int) h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                string mime = h.Header("Content-Type");
                if (string.IsNullOrEmpty(mime)) throw new BadRequestException();

                bool mimeOk = false;
                var product = Product.getProductById(pId);
                foreach (string m in Product.getMimesForProductType(product.productType)) {

                    if (m.Equals(mime)) { mimeOk = true; break; }
                }

                if (data == null || !mimeOk) throw new BadRequestException();

                var invoker = h.Authorize();

                // PERSIST

                Product.persistMedia((uint) pId, mime, data);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { h.Failure(403); }
            catch (NotFoundException) { h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { h.Failure(404); }
            catch (Exception) { h.Failure(500); }
        }

        public void UpdateProductThumbnail(string id, Stream data)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                string mime = h.Header("Content-Type");
                if (mime == null || !(mime.Equals("image/png") || mime.Equals("image/gif") || mime.Equals("image/jpeg"))) throw new BadRequestException();

                if (data == null) throw new BadRequestException();

                var invoker = h.Authorize();

                // PERSIST

                Product.persistMediaThumbnail(pId, mime, data);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { h.Failure(403); }
            catch (NotFoundException) { h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { h.Failure(404); }
            catch (Exception) { h.Failure(500); }
        }

        public void DeleteProduct(string id)
        {
            h.Failure(401);
            return;
        }

        public Stream GetProductRating(string id)
        {
            try
            {
                // AUTHORIZE

                var invoker = h.Authorize();

                // RETRIEVE RATING

                int pId = (int)h.Uint(id);

                RatingData rating = c.Convert(h.OrNull(Product.getProductById(pId).rating));

                // PRODUCE RESPONSE

                h.Success();

                return j.Json(rating);
            }
            catch (PermissionExceptions.PermissionDenied) { return h.Failure(403); }
            catch (BadRequestException) { return h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.ArgumentException) { return h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { return h.Failure(404); }
            catch (Exception) { return h.Failure(500); }
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            try
            {
                // VERIFY

                int pId;
                try { pId = (int)h.Uint(id); }
                catch(BadRequestException){ throw new NotFoundException(); }

                var invoker = h.Authorize();

                string user;
                if (invoker.IsAuth) user = ((PermissionsUtil.Invoker.Auth)invoker).Item.user;
                else throw new PermissionExceptions.PermissionDenied();

                // Rating is valid
                if(data == null || (data.score >= -5 && data.score <= 5)) throw new BadRequestException();

                // ADD RATING

                Product.rateProduct(pId, user, data.score);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { h.Failure(403); }
            catch (NotFoundException) { h.Failure(404); }
            catch (ProductExceptions.ArgumentException) { h.Failure(400); }
            catch (ProductExceptions.NoSuchProduct) { h.Failure(404); }
            catch (Exception) { h.Failure(500); }
        }

        public Stream GetProductThumbnail(string id)
        {
            try
            {
                // VERIFY

                uint pId;
                try { pId = h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = h.Authorize();

                // LOAD

                var result = Product.getMediaThumbnail(pId);

                // SIGNAL SUCCESS

                h.SetHeader("Content-Length", result.Item1.Length.ToString());
                h.Success(200, result.Item2);

                return result.Item1;
            }
            catch (BadRequestException) { return h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return h.Failure(403); }
            catch (NotFoundException) { return h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { return h.Failure(404); }
            catch (ProductExceptions.NoSuchMedia) { return h.Failure(404); }
            catch (Exception) { return h.Failure(500); } 
        }

        public Stream GetSupportedProductTypes()
        {
            try
            {
                var invoker = h.Authorize();

                h.Success();
                return j.Json(Product.getListOfProductTypes(/*invoker*/));
            }
            catch (PermissionExceptions.PermissionDenied) { return h.Failure(403); }
            catch (Exception) { return h.Failure(500); }
        }
    }
}