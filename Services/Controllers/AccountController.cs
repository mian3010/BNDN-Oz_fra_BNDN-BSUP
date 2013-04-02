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
        private Helper h;
        private JsonSerializer j;
        private CoreConverter c;

        public AccountController(Helper helper, JsonSerializer json, CoreConverter converter)
        {
            h = helper;
            j = json;
            c = converter;
        }

        public Stream GetAccounts(string types, string info, string include_banned)
        {
            try
            {
                // VALIDATE PARAMETERS

                types = h.DefaultString(types, "ACP"); // Default
                HashSet<string> fullTypes = h.ExpandAccountTypes(types);

                info = h.OneOf(info, "more", "detailed");
                bool also_banned = h.Boolean(include_banned);

                // AUTHORIZE

                var invoker = h.Authorize();

                // RETRIEVE ACCOUNTS

                // Retrieve all accounts of the selected types
                FSharpList<AccountTypes.Account> accounts = ListModule.Empty<AccountTypes.Account>();
                foreach (string type in fullTypes)
                {
                    accounts = ListModule.Append(accounts, ControlledAccount.getAllByType(invoker, type));
                }

                // Remove banned accounts if we are told so
                if (!also_banned) accounts = Account.filterBanned(accounts);

                // Include authenticated info if required
                bool alsoAuth = info.Equals("detailed");
                Dictionary<string, DateTime?> authTimes = new Dictionary<string, DateTime?>();

                if (alsoAuth)
                {
                    foreach (var a in accounts)
                    {
                        authTimes[a.user] = h.OrNulled(ControlledAccount.getLastAuthTime(invoker, a.user));
                    }
                }

                // PRODUCE RESPONSE

                AccountData[] results = h.Map(ListModule.ToArray(accounts), a => c.Convert(a, alsoAuth ? authTimes[a.user] : null));

                h.Success();

                if (info.Equals("detailed")) return j.Json(results);                                                   // All non-null returned
                if (info.Equals("more")) return j.Json(results, new string[] { "user", "email", "type", "banned" });   // Only user, email, type, and banned are returned
                if (info.Equals("username")) return j.Json(h.Map(results, a => a.user));                               // Only usernames are returned
                else throw new BadRequestException(); // Never happens
            }
            catch (BadRequestException) { return h.Failure(400); }
            catch (AccountExceptions.UnknownAccType) { return h.Failure(400); }
            catch (AccountPermissions.PermissionDenied) { return h.Failure(403); }
            catch (Exception) { return h.Failure(500); }
        }

        public Stream GetAccount(string user)
        {
            try
            {
                // VERIFY

                var invoker = h.Authorize();
                var accType = invoker.IsAuth ? ((AccountPermissions.Invoker.Auth) invoker).Item.accType : "";

                // GET DATA

                DateTime? authTime = h.OrNulled(ControlledAccount.getLastAuthTime(invoker, user));
                AccountData account = c.Convert(ControlledAccount.getByUsername(invoker, user), authTime);

                // DEFINE RETURN CONTENT

                string[] keep = {};

                if (accType.Equals("Customer")) keep = new string[] { "email", "type", "name", "address", "credits", "birth", "about" };
                else if (accType.Equals("Content Provider")) keep = new string[] { "email", "type", "name", "address" };
                else if (accType.Equals("Admin")) keep = new string[] { "email", "type", "name", "address", "credits", "birth", "about", "banned", "authenticated", "created" };
                // else client is unauthenticated and nothing is returned

                // RETURN

                h.Success();

                return j.Json(account, keep);
            }
            catch (BadRequestException) { return h.Failure(400); }
            catch (AccountPermissions.PermissionDenied) { return h.Failure(403); }
            catch (AccountExceptions.NoSuchUser) { return h.Failure(404); }
            catch (Exception) { return h.Failure(500); }
        }

        public void UpdateAccount(string user, AccountData data)
        {
            try
            {
                // VERIFY

                var invoker = h.Authorize();

                // UPDATE DATA

                bool outdated = true;
                while (outdated) {

                    try
                    {
                        var account = ControlledAccount.getByUsername(invoker, user);
                        var updated = c.Merge(account, data);
                        ControlledAccount.update(invoker, updated);

                        // If we get so far, the update went as planned, so we can quit the loop
                        outdated = false;
                    }
                    catch (AccountExceptions.OutdatedData) { /* Exception = load latest data and update based on it */ }
                }

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (AccountExceptions.BrokenInvariant) { h.Failure(400); }
            catch (AccountPermissions.PermissionDenied) { h.Failure(403); }
            catch (AccountExceptions.NoSuchUser) { h.Failure(404); }
            catch (AccountExceptions.TooLargeData) { h.Failure(413); }
            catch (Exception) { h.Failure(500); }
        }

        public void CreateAccount(string user, AccountData data)
        {
            try
            {
                // VERIFY

                var invoker = h.Authorize();

                // UPDATE DATA

                if (data.type == null) data.type = "Customer"; // Default to customer account

                var account = c.Convert(data);
                ControlledAccount.persist(invoker, account);

                // SIGNAL SUCCESS

                h.Success(204);
            }
            catch (BadRequestException) { h.Failure(400); }
            catch (AccountExceptions.BrokenInvariant) { h.Failure(400); }
            catch (AccountPermissions.PermissionDenied) { h.Failure(403); }
            catch (AccountExceptions.UserAlreadyExists) { h.Failure(409); }
            catch (AccountExceptions.TooLargeData) { h.Failure(413); }
            catch (Exception) { h.Failure(500); }
        }
    }
}