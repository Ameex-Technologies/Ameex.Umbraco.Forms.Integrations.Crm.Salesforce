using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Extension
{
    public static class ObjectExtension
    {
        public static IDictionary<string, string> AsDictionary(this object source, bool doKeyLowerCase, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance) =>
           source.GetType().GetProperties(bindingAttr).ToDictionary
           (
               propInfo => GetPropertyName(propInfo, doKeyLowerCase),
               propInfo => (propInfo.GetValue(source, null)?.ToString())
           );

        public static string ToJsonString(this object source)
        {
            return JsonConvert.SerializeObject(source, Formatting.Indented);
        }
        
        public static T ToObject<T>(this string source) where T : class
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        private static string GetPropertyName(PropertyInfo propInfo, bool isLowercase)
        {
            var jsonPropertyAttribute = ((JsonPropertyAttribute[])propInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), true)).SingleOrDefault();
            if (jsonPropertyAttribute == null)
            {
                return isLowercase ? propInfo.Name.ToLower() : propInfo.Name;
            }

            return jsonPropertyAttribute.PropertyName;
        }
    }
}
