using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentIt.Services.Controllers
{
    public class CreditsController
    {   
        private readonly Helper h;
        private readonly JsonSerializer j;
        private CoreConverter c;

        public CreditsController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            h = helper;
            j = json;
            c = converter;
        }

        public void BuyCredits(CreditsData data) {

            try
            {
                // VERIFY

                if (data == null || data.credits == 0) throw new BadRequestException();

                var invoker = h.Authorize();

                AccountTypes.Account account;
                if(invoker.IsAuth) account = ((PermissionsUtil.Invoker.Auth) invoker).Item;
                else throw new PermissionExceptions.PermissionDenied("You must be authenticated to buy credits");

                // ADD CREDITS

                ControlledCredits.purchaseCredits(invoker, account, (int) data.credits);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { h.Failure(403); }
            catch (CreditsExceptions.TooLargeData) { h.Failure(413); }
            catch (Exception) { h.Failure(500); }
        }
    }
}