using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace RentIt.Services
{
    public class JsonSerializer
    {
        private Helper h;

        public JsonSerializer(Helper helper)
        {
            h = helper;
        }

        // The types this JsonSerializer is able to handle
        private IEnumerable<Type> types = new Type[]{
                                                        typeof(TokenData),
                                                        typeof(AccountData),
                                                        typeof(AddressData),
                                                        typeof(ProductData),
                                                        typeof(PriceData),
                                                        typeof(RatingData),
                                                        typeof(MetaData),
                                                        typeof(CreditsData),
                                                        typeof(PurchaseData),
                                                        typeof(IdData)
                                                };

        // Converts an object to JSON format, leaving any null value out
        public string Json<T>(T obj)
        {
            if(!types.Contains(obj.GetType())) throw new Exception("Could not serialize given object - its class is not supported.");

            // Collect all properties to serialize to JSOn
            var properties = new Dictionary<string, string>(); 
            foreach (var p in obj.GetType().GetProperties()) {

                object value = p.GetValue(obj);

                if(value == null) continue; // Ignore null values
                
                // Convert each value to JSON representation
                if (typeof(String).IsInstanceOfType(value)) properties[p.Name] = "\"" + escape((string)value) + "\"";
                else if(typeof(uint?).IsInstanceOfType(value)) properties[p.Name] = ((uint?) value).ToString();
                else if(typeof(uint).IsInstanceOfType(value)) properties[p.Name] = ((uint) value).ToString();
                else if(typeof(bool?).IsInstanceOfType(value)) properties[p.Name] = ((bool?) value).ToString();
                else if(typeof(bool).IsInstanceOfType(value)) properties[p.Name] = ((bool) value).ToString();
                else properties[p.Name] = Json(value);
            }

            // Produce JSON

            string[] result = new string[properties.Count];

            int c = 0;
            foreach(var kv in properties)
            {
                result[c++] = "\"" + kv.Key + "\":" + kv.Value;
            }

            return "{"+h.Join(result, ",")+"}";
        }

        // Converts an object to JSON format, only including those properties which have been specified to be included
        public string Json<T>(T obj, string[] keep)
        {
            var keepSet = new HashSet<string>(keep);

            return Json(nullOutBut(obj, keepSet));
        }

        // Converts multiple objects to JSON format
        public string Json<T>(T[] objects)
        {
            var values = h.Map(objects, o => Json(o));

            return "[" + h.Join(values, ",") + "]";
        }

        // Converts multiple objects to JSON format, only including those properties which have been specified to be included
        public string Json<T>(T[] objects, string[] keep)
        {
            var keepSet = new HashSet<string>(keep);

            var values = h.Map(objects, o => Json(nullOutBut(o, keepSet)));

            return "[" + h.Join(values, ",") + "]";
        }

        // Causes any property but those specified by keep to be set to null
        private T nullOutBut<T>(T obj, HashSet<string> keep)
        {
            foreach (var p in obj.GetType().GetProperties()) {

                if (!keep.Contains(p.Name)) p.SetValue(obj, null);
            }

            return obj;
        }

        // Escapes a string, producing a string literal based on it
        private string escape(string str)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(str), writer, null);
                    return writer.ToString();
                }
            }
        }
    }
}