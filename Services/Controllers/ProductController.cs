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
        //TODO: Add catch for permission denied
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
                var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            
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
            catch (ProductExceptions.ArgumentException) { return h.Failure(400); }
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
                var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
                var user = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.user : "";

                // RETRIEVE PRODUCT

                // NB: MUST THROW EXCEPTION IF WE ARE NOT ALLOWED TO VIEW PRODUCT, BECAUSE IT IS UNPUBLISHED!
                ProductData product = c.Convert(Product.getProductById(pId));

                // PRODUCE RESPONSE

                // Normal users do not get the publish status of products
                if (!Ops.compareUsernames(user, product.owner) && !accType.Equals("Admin")) product.published = null;

                h.Success();

                return j.Json(product);
            }
            catch (BadRequestException) { return h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.ArgumentException) { return h.Failure(404); }
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
            catch (BadRequestException) { h.Failure(404); } // Only thrown if id != uint
            catch (ProductExceptions.ArgumentException) { h.Failure(400); }
            catch (ProductExceptions.NoSuchProduct) { h.Failure(404); }
            catch (ProductExceptions.TooLargeData) { h.Failure(413); }
            catch (Exception) { h.Failure(500); }
        }

        public void UpdateProductMedia(string id, Stream data)
        {

        }

        public void UpdateProductThumbnail(string id, Stream data)
        {
            try
            {
                // VERIFY

                int pId;
                try { pId = (int)h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = h.Authorize();

                // Rating is valid
                if (data == null) throw new BadRequestException();

                // ADD RATING

                Product.rateProduct(pId, user, data.score);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (NotFoundException) { h.Failure(404); }
            catch (ProductExceptions.ArgumentException) { h.Failure(400); }
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
                if (invoker.IsAuth) user = ((AccountPermissions.Invoker.Auth)invoker).Item.user;
                else throw new AccountPermissions.PermissionDenied();

                // Rating is valid
                if(data == null || (data.score >= -5 && data.score <= 5)) throw new BadRequestException();

                // ADD RATING

                Product.rateProduct(pId, user, data.score);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (NotFoundException) { h.Failure(404); }
            catch (ProductExceptions.ArgumentException) { h.Failure(400); }
            catch (ProductExceptions.NoSuchProduct) { h.Failure(404); }
            catch (Exception) { h.Failure(500); }
        }

        public Stream GetPurchasedMedia(string customer, string id)
        {

        }

        public Stream GetProductThumbnail(string id)
        {
 
        }

        public Stream GetSupportedProductTypes()
        {
            try
            {
                var invoker = h.Authorize();

                h.Success();
                return j.Json(Product.getListOfProductTypes(/*invoker*/));
            }
            // catch (ProductPermissions.PermissionDenied) { return h.Failure(403); }
            catch (Exception) { return h.Failure(500); }
        }
    }
}