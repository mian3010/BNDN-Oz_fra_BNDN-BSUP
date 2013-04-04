﻿using System;
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
        
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private CoreConverter _c;

        public ProductController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream GetProducts(string search, string type, string info, string unpublished)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            OutgoingWebResponseContext response = _h.GetResponse();

            try
            {
                FSharpList<ProductTypes.Product> products = string.IsNullOrEmpty(type) ? Product.getAll() : Product.getAllByType(type);
                Stream returnStream;
                // DEFINE RETURN CONTENT
                string[] keep = { };
                if (_h.DefaultString(info, "id").Equals("id"))
                {
                    uint[] idList = new uint[products.Length];
                    for (int i = 0; i < products.Length; i++)
                    {
                        idList[i] = (uint) products[i].id;
                    }
                    returnStream = _j.JsonArray(idList);
                } else {
                    if (info.ToLowerInvariant().Equals("more"))
                    {
                        if (accType.Equals("Customer")) keep = new string[]             { "title", "description", "type", "price", "owner"};
                        else if (accType.Equals("Content Provider")) keep = new string[]{ "title", "description", "type", "price", "owner" };
                        else if (accType.Equals("Admin")) keep = new string[]           { "title", "description", "type", "price", "owner", "published" };
                    } else if (info.ToLowerInvariant().Equals("detailed"))
                    {
                        if (accType.Equals("Customer")) keep = new string[]             { "title", "description", "type", "price", "rating", "owner", "meta" };
                        else if (accType.Equals("Content Provider")) keep = new string[]{ "title", "description", "type", "price", "rating", "owner", "meta" };
                        else if (accType.Equals("Admin")) keep = new string[]           { "title", "description", "type", "price", "rating", "owner", "meta", "published" };
                    }

                    ProductData[] returnData = new ProductData[products.Length];

                    for (int i = 0; i < products.Length; i++)
                    {
                        returnData[i] = ProductTypeToProductData(products[i]);
                    }

                    _h.GetResponse().ContentType = "text/json";
                    response.StatusCode = HttpStatusCode.NoContent;
                    returnStream = _j.Json(returnData, keep);
                }
                return returnStream;
            }
            catch (ProductExceptions.NoSuchProductType) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (ProductExceptions.ArgumentException) { response.StatusCode = HttpStatusCode.BadRequest; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }

            return null;
        }
            
        public Stream GetProduct(string id)
        {
            var invoker = _h.Authorize();
            var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.accType : "";
            var user = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth)invoker).Item.user : "";

            OutgoingWebResponseContext response = _h.GetResponse();

            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                ProductData returnData = ProductTypeToProductData(product);

                // DEFINE RETURN CONTENT
                string[] keep = { };
                if (accType.Equals("Customer")) keep = new string[] { "title", "description", "type", "price", "rating", "owner", "meta"};
                else if (accType.Equals("Admin") || Ops.compareUsernames(product.owner, user)) keep = new string[] { "title", "description", "type", "price", "rating", "owner", "meta", "published" }; // Check for ownership before check for "Provider" type, due to the fact that the owner is a provider.
                else if (accType.Equals("Content Provider")) keep = new string[] { "title", "description", "type", "price", "rating", "owner", "meta"};

                _h.GetResponse().ContentType = "text/json";
                response.StatusCode = HttpStatusCode.NoContent;
                return _j.Json(returnData, keep);
            } 
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
            return null;
        }

        public void UpdateProduct(string id, ProductData data)
        {
            var invoker = _h.Authorize();
            OutgoingWebResponseContext response = _h.GetResponse();

            try
            {
                ProductTypes.Product oldProduct = Product.getProductById(int.Parse(id));
                ProductTypes.Product newProduct = 
                    new ProductTypes.Product( 
                        data.title,
                        oldProduct.createDate,
                        data.type,
                        data.owner,
                        _h.RatingDataToRatingTypeOption(data.rating),
                        data.published == null || (bool) data.published,
                        int.Parse(id),
                        FSharpOption<string>.None, // Use the API to get the thumbnail
                        _h.MetaDataToMetaTypeMapOption(data.meta),
                        FSharpOption<string>.Some(data.description),
                        data.price.rent == null ? FSharpOption<int>.None : FSharpOption<int>.Some((int)data.price.rent),
                        FSharpOption<int>.Some((int)data.price.buy));
                Product.update(newProduct);
                response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (ProductExceptions.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (ProductExceptions.UpdateNotAllowed) { response.StatusCode = HttpStatusCode.Forbidden; }
            catch (ProductExceptions.ArgumentException) { response.StatusCode = HttpStatusCode.BadRequest; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
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