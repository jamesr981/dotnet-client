using System.Collections.Generic;
using System.Text.Json;
using System.Web;


namespace Wonde.Helpers
{
    /// <summary>
    /// Helper class to serialize/deserialize string into object or vice-versa
    /// </summary>
    internal class StringHelper
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
            { Converters = { new JsonDictionaryConverter() } };
        
        /// <summary>
        /// Converts a Json string into Key/Value pair representation of Dictionary object
        /// </summary>
        /// <param name="jsonString">The Json String to format as Object</param>
        /// <returns>Json data decoded as Key/Value pair representation of Dictionary object</returns>
        internal static Dictionary<string, object> getJsonAsDictionary(string jsonString)
        {
            if (jsonString.Trim().Length == 0)
                return null;

            return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, JsonSerializerOptions);
        }

        /// <summary>
        /// Converts an object to JsonString
        /// </summary>
        /// <param name="arrayObj">The object to be converted with its public properties</param>
        /// <returns>Json formated string</returns>
        internal static string formatObjectAsJson(object arrayObj)
        {
            if (arrayObj == null)
                return "{}";

            return JsonSerializer.Serialize(arrayObj);
        }

        internal static string buildHttpQueryString(Dictionary<string, string> data, string delimeter = "&")
        {
            string query = "";
            if(data!= null && data.Count  > 0)
            {
                foreach(KeyValuePair<string, string> kv in data)
                {
                    query += kv.Key + "=" + HttpUtility.UrlEncode(kv.Value) + delimeter;
                }
            }

            return query.Substring(0, query.Length - 1);
        }
    }
}
