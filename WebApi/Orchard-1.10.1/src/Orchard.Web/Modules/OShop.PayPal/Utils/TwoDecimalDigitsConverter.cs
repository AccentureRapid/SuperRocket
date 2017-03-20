using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace OShop.PayPal.Utils {
    public class TwoDecimalDigitsConverter : JsonConverter {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    return null;
                }
                else {
                    throw new ArgumentException(String.Format("Could not convert null value to {0}.", objectType));
                }
            }
            decimal dValue;
            if (decimal.TryParse(reader.Value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out dValue)) {
                return dValue;
            }
            else {
                throw new ArgumentException(String.Format("Could not convert {0} value to decimal.", reader.Value.ToString()));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) {
                writer.WriteNull();
            }
            else {
                writer.WriteValue(String.Format(CultureInfo.InvariantCulture, "{0:F2}", value));
            }
        }

        public override bool CanConvert(Type objectType) {
            if(objectType == typeof(decimal)){
                return true;
            }
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                return Nullable.GetUnderlyingType(objectType) == typeof(decimal);
            }
            else {
                return false;
            }
        }
    }
}