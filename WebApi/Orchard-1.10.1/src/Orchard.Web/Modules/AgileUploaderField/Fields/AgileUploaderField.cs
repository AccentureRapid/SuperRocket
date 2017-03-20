using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace AgileUploaderField.Fields
{
    public class AgileUploaderField : ContentField
    {
        public string FileNames
        {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value); }
        }

        public string AlternateText
        {
            get { return Storage.Get<string>("AlternateText"); }
            set { Storage.Set("AlternateText", value); }
        }

    }

}