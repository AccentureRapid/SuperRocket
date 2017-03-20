using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Onestop.Layouts.Models;

namespace Onestop.Layouts.Helpers {
    public static class XmlHelpers {
        public static string Attr(this XElement element, string attributeName) {
            if (element == null) return "";
            var attr = element.Attribute(attributeName);
            return attr != null ? attr.Value : "";
        }

        public static int AttrInt(this XElement element, string attributeName) {
            var attr = Attr(element, attributeName);
            if (String.IsNullOrWhiteSpace(attr)) return 0;
            int val;
            if (int.TryParse(attr, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)) return val;
            return 0;
        }

        public static DateTime AttrDate(this XElement element, string attributeName) {
            var attr = Attr(element, attributeName);
            return String.IsNullOrWhiteSpace(attr) 
                ? DateTime.MinValue
                : DateTime.Parse(attr, CultureInfo.InvariantCulture);
        }

        public static bool AttrBool(this XElement element, string attributeName) {
            var attr = Attr(element, attributeName);
            return !String.IsNullOrWhiteSpace(attr) &&
                   (attr.Equals("true", StringComparison.InvariantCultureIgnoreCase) ||
                    attr.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static Length AttrLength(this XElement element, string attributeName) {
            var attr = Attr(element, attributeName);
            if (String.IsNullOrWhiteSpace(attr)) return new Length(-1, "");
            var firstNonDigit = 0;
            while (firstNonDigit < attr.Length && !Char.IsLetter(attr[firstNonDigit]) && attr[firstNonDigit] != '%') {
                firstNonDigit++;
            }
            if (firstNonDigit < attr.Length) {
                return new Length(float.Parse(attr.Substring(0, firstNonDigit)), attr.Substring(firstNonDigit));
            }
            return new Length(float.Parse(attr), Length.Pixel);
        }

        public static string Val(this XElement element) {
            return element == null ? "" : element.Value;
        }

        public static XElement FindParentWithAttributes(this XElement element, params string[] attributeNames) {
            var parent = element.Parent;
            while (parent != null) {
                if (attributeNames.All(n => parent.Attribute(n) != null)) break;
                parent = parent.Parent;
            }
            return parent;
        }
    }
}
