using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Microsoft.FSharp.Collections;
using Services;

namespace RentIt.Services.Controllers
{
    public class CreditsController
    {   
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private readonly CoreConverter _c;

        public CreditsController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public void BuyCredits(CreditsData data) {

            try
            {
                // VERIFY

                if (data == null || data.credits == 0) throw new BadRequestException();

                var invoker = _h.Authorize();

                AccountTypes.Account account;
                if(invoker.IsAuth) account = ((PermissionsUtil.Invoker.Auth) invoker).Item;
                else throw new PermissionExceptions.PermissionDenied("You must be authenticated to buy credits");

                // ADD CREDITS

                ControlledCredits.purchaseCredits(invoker, account, (int) data.credits);

                // SIGNAL SUCCESS

                _h.Success(204);
            }
            catch (BadRequestException) { _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (CreditsExceptions.TooLargeData) { _h.Failure(413); }
            catch (Exception) { _h.Failure(500); }
        }

        public Stream GetPurchases(string customer, string purchases, string info)
        {
            try
            {

                var invoker = _h.Authorize();
                string types = _h.DefaultString(purchases, "BR");
                info = _h.DefaultString(info, "id");

                AccountTypes.Account account = ControlledAccount.getByUsername(invoker, customer);

                FSharpList<CreditsTypes.RentOrBuy> rentOrBuys;
                if (types.Equals("B"))          { rentOrBuys = ControlledCredits.getTransactionsByType(invoker, account, false); }
                else if (types.Equals("R"))     { rentOrBuys = ControlledCredits.getTransactionsByType(invoker, account, true); }
                else if (types.Equals("BR"))    { rentOrBuys = ControlledCredits.getTransactions(invoker, account); }
                else                            { return _h.Failure(400); }

                _h.Success();
                if (info.Equals("id")) return _j.Json(_h.Map(rentOrBuys, rb => rb.IsBuy ? (uint)((CreditsTypes.RentOrBuy.Buy) rb).Item.item.id : (uint)((CreditsTypes.RentOrBuy.Rent) rb).Item.item.id));
                else if (info.Equals("more")) return _j.Json(_h.Map(rentOrBuys, rb => _c.Convert(rb)));
                else { return _h.Failure(400); }
            }
            catch (CreditsExceptions.NoSuchTransaction)     { return _h.Failure(404); }
            catch (AccountExceptions.NoSuchUser)            { return _h.Failure(404); }
            catch (PermissionExceptions.AccountBanned)      { return _h.Failure(403); }
            catch (PermissionExceptions.PermissionDenied)   { return _h.Failure(403); }
            catch (Exception)                               { return _h.Failure(500); }
        }

        public Stream MakePurchases(string customer, PurchaseData[] data)
        {
            try
            {
                var invoker = _h.Authorize();
                AccountTypes.Account account = ControlledAccount.getByUsername(invoker, customer);

                List<uint> returnList = new List<uint>();

                // TODO: Only do these things, if they are known not to fail. Undo rents/buys made before error.
                foreach (PurchaseData p in data)
                {
                    ProductTypes.Product product = ControlledProduct.getProductById(invoker, (int) p.product);

                    if (!product.published) return _h.Failure(409);

                    if (p.purchased.Equals("B"))
                    {
                        var result = _h.OrNull(ControlledCredits.buyProduct(invoker, account, product));
                           
                        if(result != null) returnList.Add((uint) result.item.id);
                    }
                    else if (p.purchased.Equals("R"))
                    {
                        var result = _h.OrNull(ControlledCredits.rentProduct(invoker, account, product, 7)); // Rented products are always rented for 7 days by default

                        if (result != null) returnList.Add((uint)result.item.id);
                    }
                    else return _h.Failure(400); // Invalid letter
                }

                if (returnList.Count == 0) return _h.Failure(400); // Empty list

                _h.Success();
                return _j.Json(returnList);
            }
            catch (ProductExceptions.ArgumentException)     { return _h.Failure(400); }
            catch (CreditsExceptions.NotEnoughCredits)      { return _h.Failure(402); }
            catch (PermissionExceptions.AccountBanned)      { return _h.Failure(403); }
            catch (PermissionExceptions.PermissionDenied)   { return _h.Failure(403); }
            catch (ProductExceptions.NoSuchProduct)         { return _h.Failure(409); }
            catch (AccountExceptions.NoSuchUser)            { return _h.Failure(404); }
            catch (Exception)                               { return _h.Failure(500); }
        }

        public Stream GetPurchase(string customer, string id)
        {
            try
            {
                uint tId;
                try { tId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                CreditsTypes.RentOrBuy rentOrBuy = ControlledCredits.getTransaction(invoker, (int) tId);
                PurchaseData returnData = _c.Convert(rentOrBuy);

                _h.Success();
                return _j.Json(returnData);
            }
            catch (NotFoundException) { return _h.Failure(404); }
            catch (CreditsExceptions.NoSuchTransaction) { return _h.Failure(404); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
        }

        public Stream GetPurchasedMedia(string customer, string id)
        {
            try
            {
                // VERIFY

                uint tId;
                try { tId = _h.Uint(id); }
                catch (BadRequestException) { throw new NotFoundException(); }

                var invoker = _h.Authorize();

                // LOAD

                CreditsTypes.RentOrBuy rentOrBuy = ControlledCredits.getTransaction(invoker, (int)tId);
                PurchaseData returnData = _c.Convert(rentOrBuy);

                AccountTypes.Account account = ControlledAccount.getByUsername(invoker, customer);

                ProductTypes.Product product = ControlledProduct.getProductById(invoker, (int) returnData.product);

                if(ControlledCredits.checkAccessToProduct(invoker, account, product)) return _h.Failure(410);

                var result = Product.getMedia(returnData.product);

                // SIGNAL SUCCESS

                _h.SetHeader("Content-Length", result.Item1.Length.ToString());
                _h.Success(200, result.Item2);

                return result.Item1;
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (AccountExceptions.NoSuchUser) { return _h.Failure(404); }
            catch (NotFoundException) { return _h.Failure(404); }
            catch (ProductExceptions.NoSuchProduct) { return _h.Failure(410); }
            catch (ProductExceptions.NoSuchMedia) { return _h.Failure(410); }
            catch (Exception) { return _h.Failure(500); }
        }
    }
}