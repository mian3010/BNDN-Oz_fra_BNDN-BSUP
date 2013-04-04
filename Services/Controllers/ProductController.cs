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
        
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private CoreConverter _c;

        public ProductController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream GetProducts(string search, string types, string info, string unpublished)
        {
            try
            {
                // VALIDATE PARAMETERS

                search = _h.DefaultString(search, ""); // Default
                types = _h.DefaultString(types, ""); // Default
                HashSet<string> fullTypes = _h.ExpandProductTypes(types);

                info = _h.DefaultString(info, "id");
                info = _h.OneOf(info, "id", "more", "detailed");

                unpublished = _h.DefaultString(unpublished, "false");
                bool also_unpublished = _h.Boolean(unpublished);

                // AUTHORIZE

                var invoker = _h.Authorize();
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

                _h.Success();

                if (info.Equals("detailed")) return _j.Json(_h.Map(products, p => _c.Convert(p)), keep);
                if (info.Equals("more")) return _j.Json(_h.Map(products, p => _c.Convert(p)), keep);
                if (info.Equals("id")) return _j.Json(_h.Map(products, p => (uint)p.id));  // Only ids are returned
                else throw new BadRequestException(); // Never happens
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (ProductExceptions.ArgumentException) { return _h.Failure(400); }
            catch (Exception) { return _h.Failure(500); }
        }
            
        public Stream GetProduct(string id)
        {
            try
            {
                // AUTHORIZE

                var invoker = _h.Authorize();
                var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
                var user = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.user : "";

                // RETRIEVE PRODUCT

                // NB: MUST THROW EXCEPTION IF WE ARE NOT ALLOWED TO VIEW PRODUCT, BECAUSE IT IS UNPUBLISHED!
                ProductData product = _c.Convert(Product.getProductById((int) _h.Uint(id)));

                // PRODUCE RESPONSE

                // Normal users do not get the publish status of products
                if (!Ops.compareUsernames(user, product.owner) && !accType.Equals("Admin")) product.published = null;

                _h.Success();

                return _j.Json(product);
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (ProductExceptions.ArgumentException) { return _h.Failure(400); }
            catch (ProductExceptions.NoSuchProduct) { return _h.Failure(404); }
            catch (Exception) { return _h.Failure(500); }
        }

        public void UpdateProduct(string id, ProductData data)
        {
            try
            {
                // VERIFY

                var invoker = _h.Authorize();

                // UPDATE DATA

                bool outdated = true;
                while (outdated)
                {
                    try
                    {
                        var product = Product.getProductById((int) _h.Uint(id));
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
            catch (BadRequestException) { _h.Failure(400); }
            catch (ProductExceptions.ArgumentException) { _h.Failure(400); }
            catch (ProductExceptions.NoSuchProduct) { _h.Failure(404); }
            catch (ProductExceptions.TooLargeData) { _h.Failure(413); }
            catch (Exception) { _h.Failure(500); }
        }

        public void UpdateProductMedia(string id, Stream media)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));

                /* Gets the file extension of the uploaded file.
                 * if the MIME type is "image/jpeg" the file will be saved as "file.jpeg".
                 * It is therefore of most importance that the part after the slash is a valid file extension
                 * */
                string contentType = WebOperationContext.Current.IncomingRequest.ContentType;
                contentType = contentType.Replace(@"/", "_");

                // Set the upload path to be "WCF-folder/Owner/ID.extension"
                string filePath = string.Format("{0}Uploads\\{1}\\{2}",
                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                    product.owner, id + "." + contentType);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    media.CopyTo(fs);
                    media.Close();
                }
                //TODO: No way of telling the persistence that a product has been uploaded

                response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public void UpdateProductThumbnail(string id, Stream media)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));

                /* Gets the file extension of the uploaded file.
                 * if the MIME type is "image/jpeg" the file will be saved as "file.jpeg".
                 * It is therefore of most importance that the part after the slash is a valid file extension
                 * */
                string contentType = WebOperationContext.Current.IncomingRequest.ContentType;

                string fileType = contentType.Substring(contentType.IndexOf(@"/") + 1).ToLower();
                HashSet<string> allowedTypes = new HashSet<string> { "jpeg", "jpg", "gif", "png" };
                if (allowedTypes.Contains(fileType))
                {
                    response.StatusCode = HttpStatusCode.PreconditionFailed;
                    return;
                }

                contentType = contentType.Replace(@"/", "_");

                // Set the upload path to be "WCF-folder/Owner/ID.extension"
                string filePath = string.Format("{0}Uploads\\{1}\\thumbnails\\{2}",
                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                    product.owner, id + "." + contentType);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    media.CopyTo(fs);
                    media.Close();
                }
                //TODO: No way of telling the persistence that a product thumbnail has been uploaded
                response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public void DeleteProduct(string id)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                FileInfo productFileInfo = GetLocalProductFile(id, product.owner);
                FileInfo thumbnailFileInfo = GetLocalProductFile(id, product.owner);
                File.Delete(productFileInfo.FullName);
                File.Delete(thumbnailFileInfo.FullName);

                //TODO: No way of telling the persistence that a product has been removed

                response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public Stream GetProductRating(string id)
        {
            var invoker = _h.Authorize();
            
            OutgoingWebResponseContext response = _h.GetResponse();
            _h.GetResponse().ContentType = "text/json";
            response.StatusCode = HttpStatusCode.NotImplemented;
            return null;
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();
            response.StatusCode = HttpStatusCode.NotImplemented;
        }

        public Stream GetPurchasedMedia(string customer, string id)
        {
            var invoker = _h.Authorize();

            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                FileInfo fileInfo = GetLocalProductFile(id, product.owner);
                string contentType = fileInfo.Extension.Substring(1);
                _h.GetResponse().ContentType = contentType.Replace("-", @"/");
                response.StatusCode = HttpStatusCode.OK;

                return File.OpenRead(fileInfo.FullName);
            }
            #region exceptions
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
            #endregion
            return null;
        }

        public Stream GetProductThumbnail(string id)
        {
            var invoker = _h.Authorize();

            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                FileInfo fileInfo = GetLocalThumbnailFile(id, product.owner);
                string contentType = fileInfo.Extension.Substring(1);
                _h.GetResponse().ContentType = contentType.Replace("-", @"/");
                response.StatusCode = HttpStatusCode.OK;

                return File.OpenRead(fileInfo.FullName);
            }
            #region exceptions
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
            #endregion
            return null;
        }

        public Stream GetSupportedProductTypes()
        {
            try
            {
                var invoker = _h.Authorize();

                _h.Success();
                return _j.Json(Product.getListOfProductTypes(/*invoker*/));
            }
           // catch (ProductPermissions.PermissionDenied) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
        }

        /// <summary>
        /// Converts a ProductTypes.Product into a ProductData
        /// </summary>
        /// <param name="product">The ProductTypes.Product to convert</param>
        /// <returns>The ProductTypes.Product converted into a ProductData object</returns>
        private static ProductData ProductTypeToProductData(ProductTypes.Product product)
        {
            return new ProductData
                {
                title = product.name,
                type = product.productType,
                owner = product.owner,
                description = product.description.Equals(FSharpOption<string>.None) ? null : product.description.ToString(),
                price = new PriceData
                {
                    buy = product.buyPrice.Equals(FSharpOption<int>.None) ? 0 : uint.Parse(product.buyPrice.Value.ToString()),
                    rent = product.rentPrice.Equals(FSharpOption<int>.None) ? 0 : uint.Parse(product.rentPrice.Value.ToString())
                }
            };
        }

        /// <summary>
        /// Finds a file located on the server, by using the ID and the owner of the file
        /// </summary>
        /// <param name="id">The id of the file</param>
        /// <param name="owner">The name of the owner of the file</param>
        /// <returns>A FileInfo about the asked file</returns>
        private static FileInfo GetLocalProductFile(string id, string owner)
        {
            string folderPath = string.Format("{0}Uploads\\{1}",
                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                    owner);
            DirectoryInfo dir = new DirectoryInfo(folderPath);

            FileInfo[] files = dir.GetFiles(id + ".*");
            return files[0];
        }

        /// <summary>
        /// Finds a thumbnail located on the server, by using the ID and the owner of the file
        /// </summary>
        /// <param name="id">The id of the file</param>
        /// <param name="owner">The name of the owner of the file</param>
        /// <returns>A FileInfo about the asked file</returns>
        private static FileInfo GetLocalThumbnailFile(string id, string owner)
        {
            string folderPath = string.Format("{0}Uploads\\{1}\\thumbnails",
                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                    owner);
            DirectoryInfo dir = new DirectoryInfo(folderPath);

            FileInfo[] files = dir.GetFiles(id + ".*");
            return files[0];
        }

    }
}