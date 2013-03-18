using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;

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
               Tuple<string,DateTime> t = ControlledAuth.authenticate(user, password);
               token += t.Item1 +"\", expires: \"" +JsonUtility.dateTimeToString(t.Item2) + "\"}";
               return token;
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
                response.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
        }


        public string GetAccounts(string types, string info, bool include_banned)
        {
            try
            {
                WebHeaderCollection headers = WebOperationContext.Current.IncomingRequest.Headers;
                string tokenString = headers.Keys.Get(1);
                string[] tokenSplit = tokenString.Split(',');
                string token = tokenSplit[0].Substring(8,tokenSplit[0].Length-1);
                string date = tokenSplit[1].Substring(11, tokenSplit[1].Length - 1);

                DateTime dateTime = JsonUtility.stringToDateTime(date);
                Tuple<string, DateTime> t = new Tuple<string, DateTime>(token, dateTime);

                
                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.NotImplemented;
                return null;
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
        }


        string GetAccount(string user)
        {
            try
            {
                WebHeaderCollection headers = WebOperationContext.Current.IncomingRequest.Headers;
                if (headers.Count == 2)
                {
                    string tokenString = headers.Keys.Get(1);

                    ControlledAuth.accessAccount(tokenString);
                    var acc = ControlledAccount.getByUsername(AccountPermissions.Invoker.Unauth, user);
                    
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
                response.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
        }


        public string UpdateAccount(string user, IRentItService.AccountData data)
        {
            try
            {
                WebHeaderCollection headers = WebOperationContext.Current.IncomingRequest.Headers;
                if (headers.Count == 2)
                {
                    string tokenString = headers.Keys.Get(1);

                    ControlledAuth.accessAccount(tokenString);
                    var acc = ControlledAccount.getByUsername(AccountPermissions.Invoker.Unauth, user);

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
                response.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
        }


        string CreateAccount(string user, IRentItService.AccountData data)
        {

            try
            {
               var Uaccount =  Account.make(type, user, email, password);
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

        }

        private string getToken()
        {
            WebHeaderCollection headers = WebOperationContext.Current.IncomingRequest.Headers;
            if (headers[1].Contains("token"))
            {
                return headers[1];
            }
            else
                throw new Account.BrokenInvariant();
        }

    }
}
