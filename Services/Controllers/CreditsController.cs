using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}