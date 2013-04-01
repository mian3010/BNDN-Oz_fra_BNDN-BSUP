using System;
using System.Collections.Generic;
using System.Linq;

namespace RestClient {
  public class JsonParser {
    /// <summary>
    /// Start of a JSON encoded string
    /// </summary>
    public static string Start = "{";
    /// <summary>
    /// End of a JSON encoded string
    /// </summary>
    public static string End = "}";

    /// <summary>
    /// Convert a field to JSON. Value can either be string or dictionary of subfields.
    /// </summary>
    /// <param name="name">Name of field</param>
    /// <param name="value">String value or dictionary of subfields</param>
    /// <returns></returns>
    public static string Field(string name, object value) {
      if (value == null) throw new ArgumentNullException();
      var jsonField = "";
      if (value is Dictionary<string, object>) {
        jsonField += "\"" + name + "\":" + Start;
        jsonField = ((Dictionary<string, object>)value).Aggregate(jsonField,
                                                                    (current, dict) =>
                                                                    current + Field(dict.Key, dict.Value));
        jsonField += End;
      }
      else if (value is string) {
        jsonField += Field(name, (string)value);
      }
      else {
        throw new ArgumentException();
      }
      return jsonField;
    }

    /// <summary>
    /// Convert a field to JSON.
    /// </summary>
    /// <param name="name">String name of the field</param>
    /// <param name="value">String value of the field</param>
    /// <returns></returns>
    private static string Field(string name, string value) {
      return "\"" + name + "\":\"" + value + "\",";
    }
  }
}