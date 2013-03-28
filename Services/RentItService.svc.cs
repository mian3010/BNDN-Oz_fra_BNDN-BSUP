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
using System.Web.Services;

namespace RentIt
{

    [ServiceBehavior(Namespace = "http://rentit.itu.dk/RentIt27/WebServices/")]
    public class RentItService : IRentItService
    { 
        /// <summary>
        /// This method authenticates a given user when logging in on a client.
        /// </summary>
        /// <param name="user">The username to be autenticated</param>
        /// <param name="password">The corrosponding password</param>
        /// <returns>A token if authentication is complete, otherwise a HTTP error</returns>
        public string Authorise(string user, string password)
        {
            try
            {
                string token = "{token: \"";
                Tuple<string, DateTime> t = ControlledAuth.authenticate(user, password);
                token += t.Item1 + "\", expires: \"" + JsonUtility.dateTimeToString(t.Item2) + "\"}";
                return token;
            }
            catch (AccountPermissions.PermissionDenied)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }
            catch (AccountPermissions.AccountBanned)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            catch (Account.NoSuchUser)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Unauthorized;
                return null;
            }
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }
        }


        public string GetAccounts(string types, string info, bool include_banned)
        {
            try
            {   
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.NotImplemented;
                return null;
            }
            catch (Account.NoSuchUser)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            catch (AccountPermissions.PermissionDenied)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }
            catch (AccountPermissions.AccountBanned)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }
        }


        public string GetAccount(string user)
        {
            try
            {
                string token = getToken();
                var account = ControlledAccount.getByUsername(AccountPermissions.Invoker.Unauth, user);
                return accountToString(account);
            }
            catch (RentIt.Account.NoSuchUser)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            catch (AccountPermissions.PermissionDenied)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }
            catch (AccountPermissions.AccountBanned)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }
            catch (Account.BrokenInvariant)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }
        }


        public void UpdateAccount(string user, AccountData data)
        {
            try
            {
                string token = getToken();
                var tokenAccount = ControlledAuth.accessAccount(token);


                Dictionary<string,string> accInfo = createNewAccount(data);

                var account = ControlledAccount.getByUsername(AccountPermissions.Invoker.Unauth, user);

                AccountTypes.Account updatedAccount;

                string email = account.email;
                AccountTypes.Password password = account.password;
                FSharpOption<string> address = account.info.address.address;
                FSharpOption<int> zipcode = account.info.address.postal;
                FSharpOption<string> country = account.info.address.country;
                FSharpOption<string> name = account.info.name;
                FSharpOption<string> aboutMe = account.info.about;
                FSharpOption<DateTime> dateOfBirth = account.info.birth;
                FSharpOption<int> credits = account.info.credits;

                if(accInfo.ContainsKey("email"))
                    email = accInfo["email"];
                if (accInfo.ContainsKey("password"))
                    password = Account.Password.create("password");
                if (accInfo.ContainsKey("address"))
                    address = FSharpOption<string>.Some(accInfo["address"]);
                if(accInfo.ContainsKey("zipcode") && Convert.ToInt32(accInfo["zipcode"])!=0)
                    zipcode = FSharpOption<int>.Some(Convert.ToInt32(accInfo["zipcode"]));
                if (accInfo.ContainsKey("country"))
                    country = FSharpOption<string>.Some(accInfo["country"]);
                if (accInfo.ContainsKey("name"))
                    name = FSharpOption<string>.Some(accInfo["name"]);
                if (accInfo.ContainsKey("aboutMe"))
                    aboutMe = FSharpOption<string>.Some(accInfo["aboutMe"]);
                if (accInfo.ContainsKey("dateOfBirth"))
                    dateOfBirth = FSharpOption<DateTime>.Some(JsonUtility.stringToDateTime(accInfo["dateOfBirth"]));

                var accountAddress = new AccountTypes.Address(address, zipcode, country);
                var extraInfo = new AccountTypes.ExtraAccInfo(name, accountAddress, dateOfBirth, aboutMe, credits);
                updatedAccount = new AccountTypes.Account(user, email, password, account.created, account.banned, extraInfo, account.accType, account.version);

                updateToNewAccount(updatedAccount, tokenAccount);

            }
            catch (RentIt.Account.NoSuchUser)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.NotFound;
            }
            catch (AccountPermissions.PermissionDenied)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
            }
            catch (AccountPermissions.AccountBanned)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Forbidden;
            }
            catch (Account.BrokenInvariant)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
        }


        public void CreateAccount(string user, AccountData data)
        {
            try
            {
                var accInfo = createNewAccount(data);
                
                string accountType;
                if (accInfo.ContainsKey("accountType"))
                    accountType = accInfo["accountType"];
                else
                    throw new Account.BrokenInvariant();

                string email;
                if (accInfo.ContainsKey("email"))
                    email = accInfo["email"];
                else
                    throw new Account.BrokenInvariant();

                string password;
                if (accInfo.ContainsKey("password"))
                    password = accInfo["password"];
                else
                    throw new Account.BrokenInvariant();

                var extraInfo = createExtraInfo(accInfo);



                var newAccount = Account.make(accountType, user, email, password, extraInfo);
                string token = WebOperationContext.Current.IncomingRequest.Headers["token"];
                if (token==null)
                    ControlledAccount.persist(AccountPermissions.Invoker.Unauth, newAccount);
                else
                {
                    var tokenAccount = ControlledAuth.accessAccount(token);
                    ControlledAccount.persist(AccountPermissions.Invoker.NewAuth(tokenAccount), newAccount);
                }
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Created;

                
            }
            catch(Account.BrokenInvariant)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            catch(Account.UserAlreadyExists)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Conflict;
            }
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

        }

        /// <summary>
        /// This method is used to extract the token from the HTTP request header.
        /// </summary>
        /// <returns> The token from the header as a string</returns>
        private string getToken()
        {
            string token = WebOperationContext.Current.IncomingRequest.Headers["token"];
            if (token != null)
                return token;
            else
                throw new Account.BrokenInvariant();
        }

        private Dictionary<string, string> createNewAccount(AccountData data)
        {
            Dictionary<string, string> AccountInfo = new Dictionary<string, string>();

            PropertyInfo[] propertyInfo = data.GetType().GetProperties();
            
            foreach (PropertyInfo property in propertyInfo)
            {
                object value = property.GetValue(data, null);
                if (value != null)
                {
                    AccountInfo[property.Name] = value.ToString();
                }
            }

            return AccountInfo;
        }

        private AccountTypes.ExtraAccInfo createExtraInfo(Dictionary<string, string> info)
        {
            AccountTypes.Address accountAddress;
            
            FSharpOption<string> address;
            if (info.ContainsKey("address"))
                address = FSharpOption<string>.Some(info["address"]);
            else
                address = FSharpOption<string>.None;

            FSharpOption<int> zipcode;
            if (info.ContainsKey("zipcode")&& Convert.ToInt32(info["zipcode"])!=0)
                zipcode = FSharpOption<int>.Some(Convert.ToInt32(info["zipcode"]));
            else
                zipcode = FSharpOption<int>.None;

            FSharpOption<string> country;
            if (info.ContainsKey("country"))
                country = FSharpOption<string>.Some(info["country"]);
            else
                country = FSharpOption<string>.None;
                
            accountAddress = new AccountTypes.Address(address, zipcode, country);

            AccountTypes.ExtraAccInfo extraInfo;


            FSharpOption<string> name;
            if (info.ContainsKey("name"))
                name = FSharpOption<string>.Some(info["name"]);
            else
                name = FSharpOption<string>.None;

            FSharpOption<DateTime> dateOfBirth;
            if (info.ContainsKey("dateOfBirth"))
                dateOfBirth = FSharpOption<DateTime>.Some(JsonUtility.stringToDateTime(info["dateOfBirth"]));
            else
                dateOfBirth = FSharpOption<DateTime>.None;

            FSharpOption<string> about;
            if (info.ContainsKey("aboutMe"))
                about = FSharpOption<string>.Some(info["aboutMe"]);
            else
                about = FSharpOption<string>.None;

            FSharpOption<int> credits;
            if (info.ContainsKey("credits"))
                credits = FSharpOption<int>.Some(Convert.ToInt32(info["credits"]));
            else if (info["accountType"] == "Customer")
                credits = FSharpOption<int>.Some(0);
            else
                credits = FSharpOption<int>.None;

            extraInfo = new AccountTypes.ExtraAccInfo(name, accountAddress, dateOfBirth, about, credits);
            return extraInfo;
        }

        private string accountToString(AccountTypes.Account account)
        {
            string stringAccount = "{";

            
            stringAccount += "\"accountType: " + account.accType.ToString()+ "\", ";
            stringAccount += "\"banned: " + account.banned.ToString() + "\", ";
            stringAccount += "\"created: " + JsonUtility.dateTimeToString(account.created) + "\", ";
            stringAccount += "\"email: " + account.email + "\", ";
            stringAccount += "\"password: " + account.password + "\",";
            stringAccount += infoToString(account.info);
            stringAccount += "}";

            return stringAccount;
        }

        private string infoToString(AccountTypes.ExtraAccInfo info)
        {
            string stringInfo = "";

            if (info.name != null)
                stringInfo += "\"name: " + info.name + "\",";
            if (info.about != null)
                stringInfo += "\"aboutMe: " + info.about + "\",";
            if (info.birth != null)
                stringInfo += "\"dateOfBirth: " + info.birth + "\",";
            if (info.credits != null)
                stringInfo += "\"credits: " + info.credits + "\",";
            if (info.address.address != null)
                stringInfo += "\"address: " + info.address.address + "\",";
            if(info.address.country != null)
                stringInfo += "\"country: " + info.address.country + "\",";
            if(info.address.postal != null)
                stringInfo += "\"zipcode: " + info.address.postal + "\",";

            return stringInfo;
        }

        private void updateToNewAccount(AccountTypes.Account updatedAccount, AccountTypes.Account tokenAccount)
        {
            try
            {
                ControlledAccount.update(AccountPermissions.Invoker.NewAuth(tokenAccount), updatedAccount);
            }
            catch (Account.OutdatedData)
            {
                updateToNewAccount(updatedAccount, tokenAccount);
            }
        }
    }
}
