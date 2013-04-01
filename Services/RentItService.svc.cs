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
using System.IO;

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

        public void UpdateProductMedia(uint id, Stream media)
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

        public Stream GetProductThumbnail(uint id)
        {
            h.Failure(501);
            return null;
        }

        public void UpdateProductThumbnail(uint id, Stream media)
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

        public Stream GetPurchasedMedia(string customer, uint id)
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

        public void UpdateProviderProductMedia(string provider, uint id, Stream media)
        {
            h.Failure(501);
        }

        public void DeleteProviderProduct(string provider, uint id)
        {
            h.Failure(501);
        }

        public Stream GetProviderProductThumbnail(string provider, uint id)
        {
            h.Failure(501);
            return null;
        }

        public void UpdateProviderProductThumbnail(string provider, uint id, Stream media)
        {
            h.Failure(501);
        }

        #endregion

        #region To be refactored with Krüger...

        //public uint[] GetProducts(string search, string type, bool unpublished)
        //{
        //    // No way of getting the ID
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    response.StatusCode = HttpStatusCode.NotImplemented;
        //    return null;
        //}

        //public ProductData[] GetProducts(string search, string type, string info, bool unpublished)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    ProductData[] returnData = null;

        //    try
        //    {
        //        FSharpList<ProductTypes.Product> products = Product.getAllByType(type);
        //        returnData = new ProductData[products.Length];

        //        for (int i = 0; i < products.Length; i++)
        //        {
        //            returnData[i] = productTypeToProductData(products[i]);
        //        }

        //        response.StatusCode = HttpStatusCode.NoContent;
        //        return returnData;
        //    }
        //    catch (Product.NoSuchProductType)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Product.ArgumentException)
        //    {
        //        response.StatusCode = HttpStatusCode.BadRequest;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }

        //    return null;
        //}

        //public ProductData GetProduct(uint id)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    ProductData returnData = new ProductData();

        //    try
        //    {
        //        ProductTypes.Product product = Product.getProductById(id.ToString());

        //        returnData = productTypeToProductData(product);

        //        response.StatusCode = HttpStatusCode.NoContent;
        //        return returnData;
        //    }
        //    catch (Product.NoSuchProduct)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }
        //    return null;
        //}

        //public void UpdateProduct(uint id, ProductData data)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

        //    try
        //    {
        //        ProductTypes.Product oldProduct = Product.getProductById(id.ToString());

        //        ProductTypes.Product newProduct = new ProductTypes.Product( data.title,
        //                                                                    oldProduct.createDate,
        //                                                                    data.type,
        //                                                                    data.owner,
        //                                                                    FSharpOption<string>.Some(data.description),
        //                                                                    FSharpOption<int>.Some((int)data.price.rent),
        //                                                                    FSharpOption<int>.Some((int)data.price.buy));
        //        Product.update(newProduct);
        //        response.StatusCode = HttpStatusCode.NoContent;
        //    }
        //    catch (Product.NoSuchProduct)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Product.UpdateNotAllowed)
        //    {
        //        response.StatusCode = HttpStatusCode.Forbidden;
        //    }
        //    catch (Product.ArgumentException)
        //    {
        //        response.StatusCode = HttpStatusCode.BadRequest;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }
        //}

        //public void UpdateProductMedia(uint id, System.IO.Stream media)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    try
        //    {
        //        ProductTypes.Product product = Product.getProductById(id.ToString());

        //        /* Gets the file extension of the uploaded file.
        //         * if the MIME type is "image/jpeg" the file will be saved as "file.jpeg".
        //         * It is therefore of most importance that the part after the slash is a valid file extension
        //         * */
        //        string contentType = WebOperationContext.Current.IncomingRequest.ContentType;
        //        contentType = "." + contentType.Substring(contentType.IndexOf("/") + 1);

        //        // Set the upload path to be "WCF-folder/Owner/ID.extension"
        //        string filePath = string.Format("{0}Uploads\\{1}\\{2}",
        //                                            AppDomain.CurrentDomain.BaseDirectory,
        //                                            product.owner, id.ToString() + "_" + product.name + contentType);
        //        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        //        {
        //            media.CopyTo(fs);
        //            media.Close();
        //        }
        //        //TODO: No way of telling the persistence that a product has been uploaded

        //        response.StatusCode = HttpStatusCode.NoContent;
        //    }
        //    catch (Product.NoSuchProduct)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }
        //}

        //public void UpdateProductThumbnail(uint id, System.IO.Stream media)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    try
        //    {
        //        ProductTypes.Product product = Product.getProductById(id.ToString());

        //        /* Gets the file extension of the uploaded file.
        //         * if the MIME type is "image/jpeg" the file will be saved as "file.jpeg".
        //         * It is therefore of most importance that the part after the slash is a valid file extension
        //         * */
        //        string contentType = WebOperationContext.Current.IncomingRequest.ContentType;
        //        contentType = contentType.Substring(contentType.IndexOf("/") + 1);

        //        HashSet<string> allowedTypes = new HashSet<string> { "jpeg", "jpg", "gif", "png" };
        //        if (allowedTypes.Contains(contentType))
        //        {
        //            response.StatusCode = HttpStatusCode.PreconditionFailed;
        //            return;
        //        }

        //        contentType = "." + contentType;

        //        // Set the upload path to be "WCF-folder/Owner/ID.extension"
        //        string filePath = string.Format("{0}Uploads\\{1}\\thumbnails\\{2}",
        //                                            AppDomain.CurrentDomain.BaseDirectory,
        //                                            product.owner, id.ToString() + "_" + product.name + contentType);
        //        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        //        {
        //            media.CopyTo(fs);
        //            media.Close();
        //        }
        //        //TODO: No way of telling the persistence that a product thumbnail has been uploaded
        //        response.StatusCode = HttpStatusCode.NoContent;
        //    }
        //    catch (Product.NoSuchProduct)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }
        //}

        //public void DeleteProduct(uint id)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

        //    try
        //    {
        //        ProductTypes.Product product = Product.getProductById(id.ToString());
        //        FileInfo fileInfo = getLocalFile(id, product.owner);
        //        File.Delete(fileInfo.FullName);

        //        //TODO: No way of telling the persistence that a product has been removed

        //        response.StatusCode = HttpStatusCode.NoContent;
        //    }
        //    catch (Product.NoSuchProduct)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }
        //}

        //public RatingData GetProductRating(uint id)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    response.StatusCode = HttpStatusCode.NotImplemented;
        //    return null;
        //}

        //public void UpdateProductRating(uint id, RatingData data)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    response.StatusCode = HttpStatusCode.NotImplemented;
        //}

        //public System.IO.Stream GetPurchasedMedia(string customer, uint id)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    try
        //    {
        //        ProductTypes.Product product = Product.getProductById(id.ToString());
        //        FileInfo fileInfo = getLocalFile(id, product.owner);

        //        WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
        //        response.StatusCode = HttpStatusCode.OK;

        //        return File.OpenRead(fileInfo.FullName);
        //    }
        //    #region exceptions
        //    catch (Product.NoSuchProduct)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //    }
        //    catch (Exception)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //    }
        //    #endregion
        //    return null;
        //}

        ///// <summary>
        ///// Converts a ProductTypes.Product into a ProductData
        ///// </summary>
        ///// <param name="product">The ProductTypes.Product to convert</param>
        ///// <returns>The ProductTypes.Product converted into a ProductData object</returns>
        //private ProductData productTypeToProductData(ProductTypes.Product product)
        //{
        //    return new ProductData()
        //    {
        //        title = product.name,
        //        type = product.productType,
        //        owner = product.owner,
        //        description = product.description == FSharpOption<string>.None ? null : product.description.ToString(),
        //        price = new PriceData()
        //        {
        //            buy = product.buyPrice == FSharpOption<int>.None ? 0 : uint.Parse(product.buyPrice.Value.ToString()),
        //            rent = product.rentPrice == FSharpOption<int>.None ? 0 : uint.Parse(product.rentPrice.Value.ToString())
        //        }
        //    };
        //}

        ///// <summary>
        ///// Finds a file located on the server, by using the ID and the owner of the file
        ///// </summary>
        ///// <param name="id">The id of the file</param>
        ///// <param name="owner">The name of the owner of the file</param>
        ///// <returns>A FileInfo about the asked file</returns>
        //private FileInfo getLocalFile(uint id, string owner)
        //{
        //    string folderPath = string.Format("{0}Uploads\\{1}",
        //                                            AppDomain.CurrentDomain.BaseDirectory,
        //                                            owner);
        //    DirectoryInfo dir = new DirectoryInfo(folderPath);

        //    FileInfo[] files = dir.GetFiles(id.ToString() + ".*");
        //    return files[0];
        //}

        #endregion
    }
}
