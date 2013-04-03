using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using System.IO;

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

        public Stream Authorize(string username, string password)
        {
            try
            {
                if (username == null || password == null) throw new BadRequestException();

                Tuple<string, DateTime> t = ControlledAuth.authenticate(username, password);

                h.Success();

                return j.Json(c.Convert(t));
            }
            catch (Auth.AuthenticationFailed) { return h.Failure(401); }
            catch (AccountPermissions.AccountBanned) { return h.Failure(403); }
            catch (Exception) { return h.Failure(500); }
        }
    }
}