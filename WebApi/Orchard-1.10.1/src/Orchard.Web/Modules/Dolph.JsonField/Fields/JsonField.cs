using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Newtonsoft.Json.Linq;

namespace Dolph.JsonField.Fields
{
    public class JsonField : ContentField {

        public string Value {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value ?? String.Empty); }
        }

        public string Template {
            get { return Storage.Get<string>("Template"); }
            set { Storage.Set("Template", value ?? String.Empty); }
        }

        public Boolean? UpdateValuesOnly
        {
            get { return Storage.Get<Boolean?>("UpdateValuesOnly") ?? false; }
            set { Storage.Set("UpdateValuesOnly", value); }
        }

        public Boolean? CanEditJsonText
        {
            get { return Storage.Get<Boolean?>("CanEditJsonText") ?? false; }
            set { Storage.Set("CanEditJsonText", value); }
        }

        public dynamic DynamicObject{
            get { return (dynamic)JObject.Parse(this.Value); }
        }
    }
}