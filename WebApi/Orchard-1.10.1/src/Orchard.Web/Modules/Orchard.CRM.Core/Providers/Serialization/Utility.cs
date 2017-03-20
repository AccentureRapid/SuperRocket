using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.CRM.Core.Providers.Serialization
{
    public static class Utility
    {
        public static void WriteArray(string name, IEnumerable values, JsonWriter writer, JsonSerializer serializer)
        {
            writer.WritePropertyName(name);
            writer.WriteStartArray();

            foreach (var item in values)
            {
                if (item == null)
                {
                    writer.WriteNull();
                    continue;
                }

                if (item is IShape || item is ContentItem || JToken.FromObject(item).Type == JTokenType.Object)
                {
                    serializer.Serialize(writer, item);
                }
                else
                {
                    writer.WriteValue(item);
                }
            }

            writer.WriteEndArray();
        }

        public static void WriteProperty(string name, object value, JsonWriter writer, JsonSerializer serializer)
        {
            writer.WritePropertyName(name);

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (value is IShape || value is ContentItem || JToken.FromObject(value).Type == JTokenType.Object)
            {
                serializer.Serialize(writer, value);
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }
}