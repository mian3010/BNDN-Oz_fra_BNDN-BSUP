using System.ServiceModel.Web;

namespace Services {
  public class QueryParameters {
    public static string get(string key) {
      var queryParameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;
      var value = queryParameters[key];
      return value;
    }
  }
}