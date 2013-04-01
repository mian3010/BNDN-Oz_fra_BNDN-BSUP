using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace RentIt.Services.Controllers
{
    public class AuthenticationController
    {
        private Helper h;
        private JsonSerializer j;
        private CoreConverter c;

        public AuthenticationController(Helper helper, JsonSerializer json, CoreConverter converter) {

            h = helper;
            j = json;
            c = converter;
        }

        public string Authorize(string username, string password)
        {
            try
            {
                Tuple<string, DateTime> t = ControlledAuth.authenticate(username, password);

                h.Status(200);

                return j.Json(c.Convert(t));
            }
            catch (Auth.AuthenticationFailed) { return h.Failure(400); }
            catch (AccountPermissions.AccountBanned) { return h.Failure(403); }
            catch (Exception) { return h.Failure(500); }
        }
    }
}