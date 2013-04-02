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
        
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private CoreConverter _c;

        public ProductController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream GetProducts(string search, string type, string unpublished)
        {
            // No way of getting the ID
            OutgoingWebResponseContext response = _h.GetResponse();
            response.StatusCode = HttpStatusCode.NotImplemented;
            return null;
        }

        public Stream GetProducts(string search, string type, string info, string unpublished)
        {
            OutgoingWebResponseContext response = _h.GetResponse();

            try
            {
                FSharpList<ProductTypes.Product> products = Product.getAllByType(type);
                ProductData[] returnData = new ProductData[products.Length];

                for (int i = 0; i < products.Length; i++)
                {
                    returnData[i] = ProductTypeToProductData(products[i]);
                }

                response.StatusCode = HttpStatusCode.NoContent;
                return _j.Json(returnData);
            }
            catch (ProductExceptions.NoSuchProductType)
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }
            catch (ProductExceptions.ArgumentException)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (Exception)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return null;
        }

        public Stream GetProduct(string id)
        {
            OutgoingWebResponseContext response = _h.GetResponse();

            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                ProductData returnData = ProductTypeToProductData(product);

                response.StatusCode = HttpStatusCode.NoContent;
                return _j.Json(returnData);
            } catch (ProductExceptions.NoSuchProduct)
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }
            catch (Exception)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return null;
        }

        public void UpdateProduct(string id, ProductData data)
        {
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
                        data.rating == null ? FSharpOption<ProductTypes.Rating>.None : FSharpOption < ProductTypes.Rating >.Some(RatingDataToMetaTypeMap(data.rating)),
                        data.published == null || (bool) data.published,
                        int.Parse(id),
                        FSharpOption<string>.None, // Use the API to get the thumbnail
                        data.meta == null ? FSharpOption<FSharpMap<string, ProductTypes.Meta>>.None : FSharpOption<FSharpMap<string, ProductTypes.Meta>>.Some(MetaDataToMetaTypeMap(data.meta)),
                        FSharpOption<string>.Some(data.description),
                        data.price.rent == null ? FSharpOption<int>.None : FSharpOption<int>.Some((int)data.price.rent),
                        FSharpOption<int>.Some((int)data.price.buy));
                Product.update(newProduct);
                response.StatusCode = HttpStatusCode.NoContent;
            } catch (ProductExceptions.NoSuchProduct)
            catch (Product.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Product.UpdateNotAllowed) { response.StatusCode = HttpStatusCode.Forbidden; }
            catch (Product.ArgumentException) { response.StatusCode = HttpStatusCode.BadRequest; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public void UpdateProductMedia(string id, Stream media)
        {
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));

                /* Gets the file extension of the uploaded file.
                 * if the MIME type is "image/jpeg" the file will be saved as "file.jpeg".
                 * It is therefore of most importance that the part after the slash is a valid file extension
                 * */
                string contentType = WebOperationContext.Current.IncomingRequest.ContentType;
                contentType = "." + contentType.Substring(contentType.IndexOf("/") + 1);

                // Set the upload path to be "WCF-folder/Owner/ID.extension"
                string filePath = string.Format("{0}Uploads\\{1}\\{2}",
                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                    product.owner, id + "_" + product.name + contentType);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    media.CopyTo(fs);
                    media.Close();
                }
                //TODO: No way of telling the persistence that a product has been uploaded

                response.StatusCode = HttpStatusCode.NoContent;
            } catch (ProductExceptions.NoSuchProduct)
            catch (Product.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public void UpdateProductThumbnail(string id, Stream media)
        {
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));

                /* Gets the file extension of the uploaded file.
                 * if the MIME type is "image/jpeg" the file will be saved as "file.jpeg".
                 * It is therefore of most importance that the part after the slash is a valid file extension
                 * */
                string contentType = WebOperationContext.Current.IncomingRequest.ContentType;
                contentType = contentType.Substring(contentType.IndexOf("/") + 1);

                HashSet<string> allowedTypes = new HashSet<string> { "jpeg", "jpg", "gif", "png" };
                if (allowedTypes.Contains(contentType))
                {
                    response.StatusCode = HttpStatusCode.PreconditionFailed;
                    return;
                }

                contentType = "." + contentType;

                // Set the upload path to be "WCF-folder/Owner/ID.extension"
                string filePath = string.Format("{0}Uploads\\{1}\\thumbnails\\{2}",
                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                    product.owner, id + "_" + product.name + contentType);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    media.CopyTo(fs);
                    media.Close();
                }
                //TODO: No way of telling the persistence that a product thumbnail has been uploaded
                response.StatusCode = HttpStatusCode.NoContent;
            } catch (ProductExceptions.NoSuchProduct)
            catch (Product.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public void DeleteProduct(string id)
        {
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
            } catch (ProductExceptions.NoSuchProduct)
            catch (Product.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
        }

        public Stream GetProductRating(string id)
        {
            OutgoingWebResponseContext response = _h.GetResponse();
            response.StatusCode = HttpStatusCode.NotImplemented;
            return null;
        }

        public void UpdateProductRating(string id, RatingData data)
        {
            OutgoingWebResponseContext response = _h.GetResponse();
            response.StatusCode = HttpStatusCode.NotImplemented;
        }

        public Stream GetPurchasedMedia(string customer, string id)
        {
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                FileInfo fileInfo = GetLocalProductFile(id, product.owner);

                _h.GetResponse().ContentType = "application/octet-stream";
                response.StatusCode = HttpStatusCode.OK;

                return File.OpenRead(fileInfo.FullName);
            }
            #region exceptions
            catch (Product.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
            #endregion
            return null;
        }

        public Stream GetProductThumbnail(string id)
        {
            OutgoingWebResponseContext response = _h.GetResponse();
            try
            {
                ProductTypes.Product product = Product.getProductById(int.Parse(id));
                FileInfo fileInfo = GetLocalThumbnailFile(id, product.owner);

                _h.GetResponse().ContentType = "application/octet-stream";
                response.StatusCode = HttpStatusCode.OK;

                return File.OpenRead(fileInfo.FullName);
            }
            #region exceptions
            catch (Product.NoSuchProduct) { response.StatusCode = HttpStatusCode.NotFound; }
            catch (Exception) { response.StatusCode = HttpStatusCode.InternalServerError; }
            #endregion
            return null;
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
        /// Converts a RatingData into a ProductTypes.Rating 
        /// </summary>
        /// <param name="data">The RatingData object to convert</param>
        /// <returns>The RatingData converted into a ProductTypes.Rating</returns>
        private static ProductTypes.Rating RatingDataToMetaTypeMap(RatingData data)
        {
            return new ProductTypes.Rating(data.score, (int)data.count);
        }

        /// <summary>
        /// Converts an array of MetaData objects into an F# map,
        /// with name as the key and value as the value.
        /// </summary>
        /// <param name="dataset">The collection of MetaData to convert</param>
        /// <returns>A new FSharpMap</returns>
        private static FSharpMap<string, ProductTypes.Meta> MetaDataToMetaTypeMap(params MetaData[] dataset)
        {
            List<Tuple<string, ProductTypes.Meta>> tupleList = new List<Tuple<string, ProductTypes.Meta>>();
            foreach(MetaData data in dataset)
            {
                tupleList.Add(new Tuple<string, ProductTypes.Meta>(data.name, new ProductTypes.Meta(data.name, data.value)));
            }
            return new FSharpMap<string,ProductTypes.Meta>(tupleList);
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