using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RentIt {
  public class RentItService {
    private const string Address = "http://rentit.itu.dk/RentIt27/RentItService.svc/";

    /// <summary>
    /// Invoke a service on RentIt services
    /// </summary>
    /// <param name="method">The HTTP method to use</param>
    /// <param name="service">The service to call</param>
    /// <param name="data">The data to send with the service call</param>
    /// <param name="token">The optional token to send along</param>
    /// <returns>String that service call returns or status code if no response</returns>
    private static string InvokeService(string method, string service, string data, string token = "") {
      if (method == "GET") service = service + "?" + data;
      var address = new Uri(Address + service);

      // Create the web request
      var request = WebRequest.Create(address) as HttpWebRequest;
      //if (request == null) throw new HttpException();

      if (token != "") request.Headers.Add("token", token);

      // Set request method
      request.Method = method;
      if (method != "GET") {
        request.ContentType = "application/json";

        // Write data
        using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
          streamWriter.Write(data);
          streamWriter.Flush();
          streamWriter.Close();
        }
      }

      // Get response
      using (var response = request.GetResponse() as HttpWebResponse) {
        // Get the response stream
        //if (response == null) throw new HttpException();
        var responseStream = response.GetResponseStream();
        //if (responseStream == null) throw new HttpException();
        var reader = new StreamReader(responseStream);

        // Return response
        var responseString = reader.ReadToEnd();
        if (responseString.Length == 0) return ""+response.StatusCode;
        return responseString;
      }
    }

    #region Authenticate
    /// <summary>
    /// Method for routing calls to service authenticate
    /// </summary>
    /// <param name="username">The username to authenticate</param>
    /// <param name="password">The password for the user</param>
    /// <returns>Token string from service</returns>
    public string Authenticate(string username, string password) {
      return InvokeService("GET", "auth", "user=" + username + "&password=" + password);
    }
    #endregion
    #region Account
    /// <summary>
    /// Method for routing calls to service GetAccounts
    /// </summary>
    /// <param name="types">APC. As described in REST API</param>
    /// <param name="info">Info to include. As described in REST API</param>
    /// <param name="includeBanned">Include banned accounts? Described in REST API</param>
    /// <param name="token">Your token. Empty string for unath service call</param>
    /// <returns>JSON response</returns>
    public string GetAccounts(string types, string info, string includeBanned, string token = "") {
      return InvokeService("GET", "accounts", "types=" + types + "&info=" + info + "&include_banned=" + includeBanned, token);
    }
    /// <summary>
    /// Method for routing calls to service GetAccount
    /// </summary>
    /// <param name="username">Username of the account you wish to get</param>
    /// <param name="token">Your token. Empty string for unath service call</param>
    /// <returns>JSON response</returns>
    public string GetAccount(string username, string token = "") {
      return InvokeService("GET", "accounts/" + username, "", token);
    }
    /// <summary>
    /// Method for routing calls to service UpdateAccount
    /// </summary>
    /// <param name="username">The username of the account you wish to update</param>
    /// <param name="data">The data you wish to update. JSON</param>
    /// <param name="token">Your token. Empty string for unath service call</param>
    /// <returns>JSON response</returns>
    public string UpdateAccount(string username, Dictionary<string, object> data, string token = "") {
      var jsonData = data.Aggregate(JsonParser.Start, (current, dataIn) => current + JsonParser.Field(dataIn.Key, dataIn.Value));
      jsonData += JsonParser.End;
      return InvokeService("PUT", "accounts/" + username, jsonData, token);
    }
    /// <summary>
    /// Method for rounting calls to service CreateAccount
    /// </summary>
    /// <param name="username">The username of the new account</param>
    /// <param name="data">The data of the new account</param>
    /// <param name="token">Your token. Empty string for unath service call</param>
    /// <returns>JSON response</returns>
    public string CreateAccount(string username, Dictionary<string, object> data, string token = "") {
      var jsonData = data.Aggregate(JsonParser.Start, (current, dataIn) => current + JsonParser.Field(dataIn.Key, dataIn.Value));
      jsonData += JsonParser.End;
      return InvokeService("POST", "accounts/" + username, jsonData, token);
    }
    #endregion
  }
}