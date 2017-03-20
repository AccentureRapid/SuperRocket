using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace Onestop.Layouts.Helpers {
    public static class CollectionHelpers {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T, int> operation) {
            var i = 0;
            foreach(var item in enumerable) {
                operation(item, i);
                i++;
            }
            return enumerable;
        }

        public static HtmlString ToJson<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            var serializer = new JavaScriptSerializer();
            return new HtmlString(serializer.Serialize(dictionary));
        }
    }
}
