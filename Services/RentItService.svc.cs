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

namespace RentIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
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
                response.StatusCode = HttpStatusCode.BadRequest;
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


        public string UpdateAccount(string user, AccountData data)
        {
            try
            {
                WebHeaderCollection headers = WebOperationContext.Current.IncomingRequest.Headers;
                if (headers.Count == 2)
                {
                    string tokenString = headers.Keys.Get(1);

                    ControlledAuth.accessAccount(tokenString);
                    var acc = ControlledAccount.getByUsername(AccountPermissions.Invoker.Unauth, user);

                    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                    response.StatusCode = HttpStatusCode.NotImplemented;
                    return null;
                }
                else
                    throw new AccountPermissions.AccountBanned(); //returns bad request error
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
            }
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }
        }


        public string CreateAccount(string user, AccountData data)
        {
            try
            {
                var accInfo = createNewAccount(data);
                var extraInfo = createExtraInfo(accInfo);
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
            catch (Exception)
            {
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }

        }

        /// <summary>
        /// This method is used to extract the token from the HTTP request header.
        /// </summary>
        /// <returns> The token from the header as a string</returns>
        private string getToken()
        {
            WebHeaderCollection headers = WebOperationContext.Current.IncomingRequest.Headers;
            foreach (string header in headers)
            {
                if(header.Contains("token"))
                {
                    return header;
                }
                else
                    throw new Account.BrokenInvariant();
            }
            return null;
        }

        private Dictionary<string, string> createNewAccount(AccountData data)
        {
            Dictionary<string, string> AccountInfo = new Dictionary<string, string>();

            PropertyInfo[] propertyInfo = data.GetType().GetProperties();
            
            foreach (PropertyInfo property in propertyInfo)
            {
                object value = property.GetValue(this, null);
                if (value != null)
                {
                    AccountInfo[property.Name] = value.ToString();
                }
            }

            return AccountInfo;
        }

        private AccountTypes.ExtraAccInfo createExtraInfo(Dictionary<string, string> info)
        {
            string address = info["address"];
            int zipcode = Convert.ToInt32(info["zipcode"]);
            string country = info["country"];

            if (info.ContainsKey("address") && info.ContainsKey("zipcode") && info.ContainsKey("country"))
            {
                AccountTypes.Address accountAddress = new AccountTypes.Address(address, Option(zipcode), country);
            }
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
    }
}
