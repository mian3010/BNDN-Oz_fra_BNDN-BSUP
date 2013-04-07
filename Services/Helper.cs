using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using Microsoft.FSharp.Core;
using System.IO;

namespace RentIt.Services
{
    /// <summary>
    /// Various helper methods
    /// </summary>
    public class Helper
    {

        #region HTTP Helpers

        /// <summary>
        /// Outgoing HTTP response entity
        /// </summary>
        /// <returns>Read what it says, dummy</returns>
        public OutgoingWebResponseContext GetResponse()
        {
            return WebOperationContext.Current.OutgoingResponse;
        }

        /// <summary>
        /// The status code object for a particular status code
        /// </summary>
        /// <param name="code">The HTTP status code to retrieve an representation of</param>
        /// <returns></returns>
        public HttpStatusCode Status(uint code)
        {
            #region switch over HTTP Status code
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
            #endregion
        }

        /// <summary>
        /// Sets the status code for the outgoing response
        /// </summary>
        /// <param name="code">The HTTP status code to set</param>
        public void SetStatus(uint code)
        {
            GetResponse().StatusCode = Status(code);
        }

        /// <summary>
        /// Retrieves the text representation of a header of the incoming request.
        /// </summary>
        /// <param name="name">Name of header to retrieve value of. Null if not available.</param>
        /// <returns>Header value of header name</returns>
        public string Header(string name)
        {
            return WebOperationContext.Current.IncomingRequest.Headers[name]; // null if it is not set
        }

        /// <summary>
        /// Sets a header of the outgoing response
        /// </summary>
        /// <param name="name">Name of the header to set</param>
        /// <param name="value">Value of the header to set</param>
        public void SetHeader(string name, string value)
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Set(name, value);
        }

        /// <summary>
        /// Shorthand for setting status code and response content type in a single method call
        /// </summary>
        /// <param name="status">Status code to set. Defaults to 200 OK</param>
        /// <param name="responseType">Response type to set. Defaults to application/json</param>
        public void Success(uint status=200, string responseType="application/json")
        {
            SetStatus(status);
            GetResponse().ContentType = responseType;
        }

        /// <summary>
        /// Shorthand for signaling a failure, setting the reponse status code to some error code (HTTP status code)
        /// </summary>
        /// <param name="statusCode">Status code of the failure</param>
        /// <returns>null</returns>
        public Stream Failure(uint statusCode)
        {
            GetResponse().StatusCode = Status(statusCode);

            return null;
        }

        #endregion

        #region API Helpers

        /// <summary>
        /// Authenticates the client using the Token header, throwing Permission Exceptions as needed if the authentication fails
        /// </summary>
        /// <returns>Invoker object representing the client to use for calls to the Controlled modules of the backend</returns>
        public PermissionsUtil.Invoker Authorize()
        {
            return Authorize(Header("Token"));
        }

        /// <summary>
        /// Authenticates the client using a custom token string, throwing Permission Exceptions as needed if the authentication fails
        /// </summary>
        /// <returns>Invoker object representing the client to use for calls to the Controlled modules of the backend</returns>
        public PermissionsUtil.Invoker Authorize(string token)
        {
            try
            {
                return token == null    ? PermissionsUtil.Invoker.Unauth 
                                        : PermissionsUtil.Invoker.NewAuth(ControlledAuth.accessAccount(token));
            }
            catch (PermissionExceptions.AccountBanned) { throw new PermissionExceptions.PermissionDenied("Account is banned"); }
            catch (AuthExceptions.IllegalToken) { throw new PermissionExceptions.PermissionDenied("Token is illegal"); }
            catch (AuthExceptions.TokenExpired) { throw new PermissionExceptions.PermissionDenied("Token is expired"); }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Returns the given value, unless its null or empty in which case the specified default value is returned.
        /// </summary>
        /// <param name="value">value to return if not null or empty</param>
        /// <param name="def">alternative value to return</param>
        /// <returns>value if non-null, non-empty, otherwise def</returns>
        public string DefaultString(string value, string def)
        {

            if (string.IsNullOrEmpty(value)) return def;
            else return value;
        }

        /// <summary>
        /// Checks if a specified string value is one of the strings found in the options array
        /// </summary>
        /// <param name="value">The value to check whether is contained inside the options array</param>
        /// <param name="options">The possible values value may be</param>
        /// <returns>value, if it is found in the options array</returns>
        /// <exception cref="BadRequestException">If the specified string is not found in the options array</exception>
        public string OneOf(string value, params string[] options)
        {
            if (options.Contains(value)) return value;
            else throw new BadRequestException();
        }

        /// <summary>
        /// Parses a boolean in string form to its normal form.
        /// </summary>
        /// <param name="value">The value to parse into a boolean</param>
        /// <returns>The value as boolean</returns>
        /// <exception cref="BadRequestException">If the specified string not either "true" or "false"</exception>
        public bool Boolean(string value)
        {
            return System.Boolean.Parse(OneOf(value, "true", "false"));
        }

        /// <summary>
        /// Parses a uint in string form to its normal form.
        /// </summary>
        /// <param name="value">The value to parse into a uint</param>
        /// <returns>The value as uint</returns>
        /// <exception cref="BadRequestException">If the specified string not a uint</exception>
        public uint Uint(string value)
        {
            try
            {
                return UInt32.Parse(value);
            }
            catch (Exception) {  throw new BadRequestException(); }
        }

        /// <summary>
        /// Converts a string of characters into a set where each character is replaced by the account type it represents
        /// </summary>
        /// <param name="types">A string containing any subset of the characters A, C, and P</param>
        /// <returns>A hashmap where each character has been expanded into its full account type form: Admin, content Provider, Customer</returns>
        /// <exception cref="BadRequestException">If other characters than A, C, and P is given within the types string</exception>
        public HashSet<string> ExpandAccountTypes(string types)
        {

            HashSet<string> controlSet = new HashSet<string>();
            foreach (char c in types) controlSet.Add(c.ToString());
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

        /// <summary>
        /// Parses a string of product types separated by '|' into a set of each product type apparent
        /// </summary>
        /// <param name="types">The string of product types separated by '|'</param>
        /// <returns>A set of the product types separated, minus any empty product type strings</returns>
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

        #endregion

        #region Other

        /// <summary>
        /// Applies a function to each element of an IEnumerable, producing an array of the results
        /// Any produces null value is ignored
        /// </summary>
        /// <typeparam name="A">The type to convert from</typeparam>
        /// <typeparam name="B">The type to convert to</typeparam>
        /// <param name="input">Input IEnumrable to map from</param>
        /// <param name="func">The function to apply to each value of the input</param>
        /// <returns>An array of the result values, minus any null values produced by the func</returns>
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

        /// <summary>
        /// Produces a string of all elements in an IEnumerable, where the ToString() value of each element is separated by a custom delimiter.
        /// Any null values of the input IEnumerable is ignored.
        /// </summary>
        /// <typeparam name="T">Type of elements in the input IEnumerable</typeparam>
        /// <param name="input">The IEnumerable to join into a string</param>
        /// <param name="delimiter">The string to separate the string representations of the input with</param>
        /// <returns></returns>
        public string Join<T>(IEnumerable<T> input, string delimiter)
        {
            string result = "";

            foreach (T e in input)
            {
                if (e == null) continue;

                result += e + delimiter;
            }

            var l = result.Length - delimiter.Length;
            return result.Substring(0, l == 0 ? 0 : l);
        }

        #endregion

        #region Null Handlers

        /// <summary>
        /// Converts an Option of Some(T)/None to T/null
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="option">The Option value to convert</param>
        /// <returns>The value of Some(T) or null, if it was None</returns>
        public T OrNull<T>(FSharpOption<T> option) where T : class
        {
            try { return option.Value; }
            catch (NullReferenceException) { return null; }
        }

        /// <summary>
        /// Converts an Option of Some(A)/None to B/null, where B is the output of applying a function to A
        /// </summary>
        /// <typeparam name="A">Type of value</typeparam>
        /// <typeparam name="B">Type of output value</typeparam>
        /// <param name="option">The Option value to convert</param>
        /// <returns>The value of func(Some(T)) or null, if it was None</returns>
        public B OrNull<A, B>(FSharpOption<A> option, Func<A, B> func) where B : class
        {
            try { return func(option.Value); }
            catch (NullReferenceException) { return null; }
        }

        /// <summary>
        /// Converts an Option of Some(A)/None to B/null, where B is the output of applying a function to A
        /// </summary>
        /// <typeparam name="A">Type of value</typeparam>
        /// <typeparam name="B">Type of output value</typeparam>
        /// <param name="option">The Option value to convert</param>
        /// <returns>The value of func(Some(T)) or null, if it was None</returns>
        public B OrNull<A, B>(Nullable<A> option, Func<A, B> func)  where A : struct
                                                                    where B : class
        {
            return option.HasValue ? func(option.Value) : null;
        }

        /// <summary>
        /// Converts an Option of Some(T)/None to T/null
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="option">The Option value to convert</param>
        /// <returns>The value of Some(T) or null, if it was None</returns>
        public T? OrNulled<T>(FSharpOption<T> option) where T : struct
        {
            try { return option.Value; }
            catch (NullReferenceException) { return null; }
        }

        /// <summary>
        /// Converts an Option of Some(A)/None to B/null, where B is the output of applying a function to A
        /// </summary>
        /// <typeparam name="A">Type of value</typeparam>
        /// <typeparam name="B">Type of output value</typeparam>
        /// <param name="option">The Option value to convert</param>
        /// <returns>The value of func(Some(T)) or null, if it was None</returns>
        public B? OrNulled<A, B>(FSharpOption<A> option, Func<A, B> func) where B : struct
        {
            try { return func(option.Value); }
            catch (NullReferenceException) { return null; }
        }

        /// <summary>
        /// Converts a value, which might be null, to an Option of the value
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to to convert to the Option type</param>
        /// <returns>An Option of the value, Some(value) if value is not null, otherwise None</returns>
        public FSharpOption<T> ToOption<T>(T value)
        {
            return value == null ? FSharpOption<T>.None : FSharpOption<T>.Some(value);
        }

        /// <summary>
        /// Converts a value, which might be null, to an Option of the value
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to to convert to the Option type</param>
        /// <returns>An Option of the value, Some(value) if value is not null, otherwise None</returns>
        public FSharpOption<T> ToOption<T>(T? value) where T:struct
        {
            return value == null ? FSharpOption<T>.None : FSharpOption<T>.Some(value.Value);
        }

        #endregion
    }
}