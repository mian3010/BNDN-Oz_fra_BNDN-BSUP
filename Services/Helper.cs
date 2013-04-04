using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;
using Microsoft.FSharp.Core;
using System.IO;

namespace RentIt.Services
{
    public class Helper
    {
        ////// HTTP Helpers

        public OutgoingWebResponseContext GetResponse()
        {

            return WebOperationContext.Current.OutgoingResponse;
        }

        public HttpStatusCode Status(uint code)
        {

            OutgoingWebResponseContext response = GetResponse();

            switch (code)
            {

                case 100:
                    return HttpStatusCode.Continue;
                case 101:
                    return HttpStatusCode.SwitchingProtocols;
                case 200:
                    return HttpStatusCode.OK;
                case 201:
                    return HttpStatusCode.Created;
                case 202:
                    return HttpStatusCode.Accepted;
                case 203:
                    return HttpStatusCode.NonAuthoritativeInformation;
                case 204:
                    return HttpStatusCode.NoContent;
                case 205:
                    return HttpStatusCode.ResetContent;
                case 206:
                    return HttpStatusCode.PartialContent;
                case 300:
                    return HttpStatusCode.MultipleChoices;
                case 301:
                    return HttpStatusCode.MovedPermanently;
                case 302:
                    return HttpStatusCode.Found;
                case 303:
                    return HttpStatusCode.SeeOther;
                case 304:
                    return HttpStatusCode.NotModified;
                case 305:
                    return HttpStatusCode.UseProxy;
                case 307:
                    return HttpStatusCode.TemporaryRedirect;
                case 400:
                    return HttpStatusCode.BadRequest;
                case 401:
                    return HttpStatusCode.Unauthorized;
                case 402:
                    return HttpStatusCode.PaymentRequired;
                case 403:
                    return HttpStatusCode.Forbidden;
                case 404:
                    return HttpStatusCode.NotFound;
                case 405:
                    return HttpStatusCode.MethodNotAllowed;
                case 406:
                    return HttpStatusCode.NotAcceptable;
                case 407:
                    return HttpStatusCode.ProxyAuthenticationRequired;
                case 408:
                    return HttpStatusCode.RequestTimeout;
                case 409:
                    return HttpStatusCode.Conflict;
                case 410:
                    return HttpStatusCode.Gone;
                case 411:
                    return HttpStatusCode.LengthRequired;
                case 412:
                    return HttpStatusCode.PreconditionFailed;
                case 413:
                    return HttpStatusCode.RequestEntityTooLarge;
                case 414:
                    return HttpStatusCode.RequestUriTooLong;
                case 415:
                    return HttpStatusCode.UnsupportedMediaType;
                case 416:
                    return HttpStatusCode.RequestedRangeNotSatisfiable;
                case 417:
                    return HttpStatusCode.ExpectationFailed;
                case 500:
                    return HttpStatusCode.InternalServerError;
                case 501:
                    return HttpStatusCode.NotImplemented;
                case 502:
                    return HttpStatusCode.BadGateway;
                case 503:
                    return HttpStatusCode.ServiceUnavailable;
                case 504:
                    return HttpStatusCode.GatewayTimeout;
                case 505:
                    return HttpStatusCode.HttpVersionNotSupported;
                default:
                    throw new Exception("Illegal HTTP status code");
            }
        }

        public void SetStatus(uint code)
        {
            GetResponse().StatusCode = Status(code);
        }

        public string Header(string name)
        {
            return WebOperationContext.Current.IncomingRequest.Headers[name]; // null if it is not set
        }

        public void SetHeader(string name, string value)
        {
            WebOperationContext.Current.IncomingRequest.Headers.Set(name, value);
        }

        public void Success(uint status=200, string responseType="application/json")
        {
            SetStatus(status);
            GetResponse().ContentType = responseType;
        }

        public Stream Failure(uint statusCode)
        {
            GetResponse().StatusCode = Status(statusCode);

            return null;
        }

        ////// API Helpers

        public AccountPermissions.Invoker Authorize()
        {
            return Authorize(Header("Token"));
        }

        public AccountPermissions.Invoker Authorize(string token)
        {
            try
            {
                if (token == null) return AccountPermissions.Invoker.Unauth;
                else return AccountPermissions.Invoker.NewAuth(ControlledAuth.accessAccount(token));
            }
            catch (AccountPermissions.AccountBanned) { throw new AccountPermissions.PermissionDenied("Account is banned"); }
            catch (Auth.Token.IllegalToken) { throw new AccountPermissions.PermissionDenied("Token is illegal"); }
            catch (Auth.Token.TokenExpired) { throw new AccountPermissions.PermissionDenied("Token is expired"); }
        }

        ////// Validation

        public string DefaultString(string value, string def)
        {

            if (string.IsNullOrEmpty(value)) return def;
            else return value;
        }

        public string OneOf(string value, params string[] options)
        {
            if (options.Contains(value)) return value;
            else throw new BadRequestException();
        }

        public bool Boolean(string value)
        {
            return System.Boolean.Parse(OneOf(value, "true", "false"));
        }

        public uint Uint(string value)
        {
            try
            {
                return System.UInt32.Parse(value);
            }
            catch (Exception) {  throw new BadRequestException(); }
        }

        public HashSet<string> ExpandAccountTypes(string types)
        {

            HashSet<string> controlSet = new HashSet<string>();
            foreach (char c in types.ToCharArray()) controlSet.Add(c.ToString());
            if (!controlSet.IsSubsetOf(new string[] { "A", "C", "P" })) throw new BadRequestException(); // Only A, C, and P is allowed

            HashSet<string> resultSet = new HashSet<string>();

            foreach (string s in controlSet)
            {

                if (s.Equals("A")) resultSet.Add("Admin");
                else if (s.Equals("C")) resultSet.Add("Customer");
                else if (s.Equals("P")) resultSet.Add("Content Provider");
            }

            return resultSet;
        }

        public HashSet<string> ExpandProductTypes(string types)
        {
            if (string.IsNullOrEmpty(types)) return new HashSet<string>();

            HashSet<string> result = new HashSet<string>();

            foreach (string t in types.Split('|')) {

                if (t.Length == 0) continue;
                result.Add(t);
            }

            return result;
        }

        ////// Other

        public B[] Map<A, B>(IEnumerable<A> input, Func<A, B> func)
        {
            LinkedList<B> list = new LinkedList<B>();
            
            int c = 0;
            foreach (A i in input)
            {
                B temp = func(i);

                if (temp != null){
                    
                    list.AddFirst(temp);
                    c++;
                }
            }

            B[] result = new B[c];

            c = 0;
            foreach (B b in list) result[c++] = b;

            return result;
        }

        public string Join<T>(IEnumerable<T> input, string delimiter)
        {
            string result = "";

            foreach (T e in input)
            {
                if (e == null) continue;

                result += e.ToString() + delimiter;
            }

            return result.Substring(0, result.Length-delimiter.Length);
        }

        /////// Null handlers

        #region Null Converters

        public T OrNull<T>(FSharpOption<T> option) where T : class
        {
            try { return option.Value; }
            catch (NullReferenceException) { return null; }
        }

        public B OrNull<A, B>(FSharpOption<A> option, Func<A, B> func) where B : class
        {
            try { return func(option.Value); }
            catch (NullReferenceException) { return null; }
        }

        public B OrNull<A, B>(Nullable<A> option, Func<A, B> func)  where A : struct
                                                                    where B : class
        {
            if (option.HasValue) return func(option.Value);
            else return null;
        }

        public T? OrNulled<T>(FSharpOption<T> option) where T : struct
        {
            try { return option.Value; }
            catch (NullReferenceException) { return null; }
        }

        public B? OrNulled<A, B>(FSharpOption<A> option, Func<A, B> func) where B : struct
        {
            try { return func(option.Value); }
            catch (NullReferenceException) { return null; }
        }

        #endregion
    }
}