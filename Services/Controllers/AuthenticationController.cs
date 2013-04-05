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
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private readonly CoreConverter _c;

        public AuthenticationController(Helper helper, JsonSerializer json, CoreConverter converter) {

            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream Authorize(string username, string password)
        {
            try
            {
                if (username == null || password == null) throw new BadRequestException();

                Tuple<string, DateTime> t = ControlledAuth.authenticate(username, password);

                _h.Success();

                return _j.Json(_c.Convert(t));
            }
            catch (Auth.AuthenticationFailed) { return _h.Failure(401); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
        }
    }
}