using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RentIt.AuthService
{
    [ServiceContract]
    public interface IAuthenticationServiceContract
    {
        /// <summary>
        /// Returns an authentication token to be used when authenticating with other RentIt services.
        /// The authentication token is bound to an expiration date, which is returned along with the token.
        /// 
        /// To authenticate the client must pass a valid username/password pair to the service.
        /// If no account with the passed username is registered or the password of the account does not match the passed password, a fault exception is raised with the fault code "INVALID_CREDENTIALS"s
        /// 
        /// If the passed credentials matches an account, an authentication token and its expiration date/time is returned as newline-separated string.
        /// The expiration date/time is given in UTC using the following format: yyyy-MM-dd HH:mm:ss zzz
        /// 
        /// Example return value from a successful Authenticate invokation:
        /// 
        /// MDAzNjEyMzQ3OCB8IDIwMTMtMDMtMjEgMTk6NDU6MjkgKzAxOjAwIHwgUGhpbGlw
        /// 2013-03-21 19:45:29 +01:00
        /// </summary>
        /// <param name="user">The username of the user who wishes two obtain an authentication token</param>
        /// <param name="password">The password associated with the given username</param>
        /// <returns>An authentication token and its expiration date and time, separated by a newline</returns>
        [OperationContract]
        string Authenticate(string user, string password);
    }
}
