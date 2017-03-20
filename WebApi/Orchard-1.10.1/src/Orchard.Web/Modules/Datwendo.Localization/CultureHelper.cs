using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Datwendo.Localization
{
    static class CultureHelper
    {
        public static string GetSpecificOrNeutralCulture(IEnumerable<string> supportedCultures, string cultureName)
        {
            try
            {
                var ci = !string.IsNullOrEmpty(cultureName) ? CultureInfo.GetCultureInfo(cultureName) : null;
                var nci = ci != null && !ci.IsNeutralCulture ? ci.Parent : ci;

                if (ci != null && supportedCultures.Contains(ci.Name, StringComparer.InvariantCultureIgnoreCase)) return ci.Name;

                if (nci != null && ci != nci && supportedCultures.Contains(nci.Name, StringComparer.InvariantCultureIgnoreCase)) return nci.Name;

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static CultureInfo ParseCultureInfo(this string cultureName)
        {
            try
            {
                return new CultureInfo(cultureName);
            }
            catch
            {
                return null;
            }
        }

        public static string UpperFirst(this string text)
        {
            return char.ToUpper(text[0]) +
                ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
        }
    }
}
