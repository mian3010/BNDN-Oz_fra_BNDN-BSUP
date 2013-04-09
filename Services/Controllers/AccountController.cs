using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using System.IO;

namespace RentIt.Services.Controllers
{
    public class AccountController
    {
        private readonly Helper _h;
        private readonly JsonSerializer _j;
        private readonly CoreConverter _c;

        public AccountController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            _h = helper;
            _j = json;
            _c = converter;
        }

        public Stream GetAccounts(string types, string info, string includeBanned)
        {
            try
            {
                // VALIDATE PARAMETERS

                types = _h.DefaultString(types, "ACP"); // Default
                HashSet<string> fullTypes = _h.ExpandAccountTypes(types);

                info = _h.DefaultString(info, "username");
                info = _h.OneOf(info, "username", "more", "detailed");

                includeBanned = _h.DefaultString(includeBanned, "false");
                bool alsoBanned = _h.Boolean(includeBanned);

                // AUTHORIZE

                var invoker = _h.Authorize();

                // RETRIEVE ACCOUNTS

                // Retrieve all accounts of the selected types
                FSharpList<AccountTypes.Account> accounts = ListModule.Empty<AccountTypes.Account>();
                foreach (string type in fullTypes)
                {
                    accounts = ListModule.Append(accounts, ControlledAccount.getAllByType(invoker, type));
                }

                // Remove banned accounts if we are told so
                if (!alsoBanned) accounts = Account.filterBanned(accounts);

                // Include authenticated info if required
                bool alsoAuth = info.Equals("detailed");
                Dictionary<string, DateTime?> authTimes = new Dictionary<string, DateTime?>();

                if (alsoAuth)
                {
                    foreach (var a in accounts)
                    {
                        authTimes[a.user] = _h.OrNulled(ControlledAccount.getLastAuthTime(invoker, a.user));
                    }
                }

                // PRODUCE RESPONSE

                AccountData[] results = _h.Map(accounts, a => _c.Convert(a, alsoAuth ? authTimes[a.user] : null));

                _h.Success();

                if (info.Equals("detailed")) return _j.Json(results);                                                   // All non-null returned
                if (info.Equals("more")) return _j.Json(results, new [] { "user", "email", "type", "banned" });   // Only user, email, type, and banned are returned
                if (info.Equals("username")) return _j.Json(_h.Map(results, a => a.user));                               // Only usernames are returned
                throw new BadRequestException(); // Never happens
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (AccountExceptions.UnknownAccType) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500); }
        }

        public Stream GetAccount(string user)
        {
            try
            {
                // VERIFY

                var invoker = _h.Authorize();
                var accType = invoker.IsAuth ? ((PermissionsUtil.Invoker.Auth) invoker).Item.accType : "";

                // GET DATA

                DateTime? authTime = _h.OrNulled(ControlledAccount.getLastAuthTime(invoker, user));
                AccountData account = _c.Convert(ControlledAccount.getByUsername(invoker, user), authTime);

                // DEFINE RETURN CONTENT

                string[] keep = {};

                if (accType.Equals("Customer")) keep = new [] { "email", "type", "name", "address", "postal", "country", "credits", "birth", "about" };
                else if (accType.Equals("Content Provider")) keep = new [] { "email", "type", "name", "address", "postal", "country" };
                else if (accType.Equals("Admin")) keep = new [] { "email", "type", "name", "address", "postal", "country", "credits", "birth", "about", "banned", "authenticated", "created" };
                // else client is unauthenticated and nothing is returned

                // RETURN

                _h.Success();

                return _j.Json(account, keep);
            }
            catch (BadRequestException) { return _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (AccountExceptions.NoSuchUser) { return _h.Failure(404); }
            catch (Exception) { return _h.Failure(500); }
        }

        public void UpdateAccount(string user, AccountData data)
        {
            try
            {
                // VERIFY

                var invoker = _h.Authorize();

                // UPDATE DATA

                bool outdated = true;
                while (outdated) {

                    try
                    {
                        var account = ControlledAccount.getByUsername(invoker, user);
                        var updated = _c.Merge(account, data);
                        ControlledAccount.update(invoker, updated);

                        // If we get so far, the update went as planned, so we can quit the loop
                        outdated = false;
                    }
                    catch (AccountExceptions.OutdatedData) { /* Exception = load latest data and update based on it */ }
                }

                // SIGNAL SUCCESS

                _h.Success(204);
            }
            catch (BadRequestException) { _h.Failure(400); }
            catch (AccountExceptions.BrokenInvariant) { _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (AccountExceptions.NoSuchUser) { _h.Failure(404); }
            catch (AccountExceptions.TooLargeData) { _h.Failure(413); }
            catch (Exception) { _h.Failure(500); }
        }

        public void CreateAccount(string user, AccountData data)
        {
            try
            {
                // VERIFY

                if (data == null || string.IsNullOrEmpty(data.email) || string.IsNullOrEmpty(data.password)) throw new BadRequestException();

                var invoker = _h.Authorize();

                // CREATE ACCOUNT

                data.user = user; // name must be username

                if (data.type == null) data.type = "Customer"; // Default to customer account
                if (data.type.Equals("Customer") && data.credits == null) data.credits = 0;

                var account = _c.Convert(data);
                ControlledAccount.persist(invoker, account);

                // SIGNAL SUCCESS

                _h.SetHeader("Location", "/accounts/"+user);
                _h.Success(201);
            }
            catch (BadRequestException) { _h.Failure(400); } // TODO: Should also be returned for too long usernames, instead of 413 
            catch (AccountExceptions.BrokenInvariant) { _h.Failure(400); }
            catch (PermissionExceptions.PermissionDenied) { _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { _h.Failure(403); }
            catch (AccountExceptions.UnknownAccType) { _h.Failure(400); }
            catch (AccountExceptions.UserAlreadyExists) { _h.Failure(409); }
            catch (AccountExceptions.TooLargeData) { _h.Failure(413); }
            catch (Exception) { _h.Failure(500); }
        }

        public Stream GetAcceptedCountries() {

            try
            {
                var invoker = _h.Authorize();

                _h.Success();
                return _j.Json(ControlledAccount.getAcceptedCountries(invoker));
            }
            catch (PermissionExceptions.PermissionDenied) { return _h.Failure(403); }
            catch (PermissionExceptions.AccountBanned) { return _h.Failure(403); }
            catch (Exception) { return _h.Failure(500);  }
        }
    }
}
