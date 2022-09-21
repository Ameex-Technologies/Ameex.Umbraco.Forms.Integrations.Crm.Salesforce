using System.Collections.Generic;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Extension
{
    public static class CommonExtension
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNull(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> source)
        {
            return source == null || source.Count == 0;
        }
    }
}
