using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onestop.Layouts.Models;

namespace Onestop.Layouts.Helpers {
	public static class HtmlHelpers {
		public static string BuildStyle(this IDictionary<string, string> styles) {
			return string.Join(";", styles.Select(kvp => kvp.Key + ":" + kvp.Value));
		}

	    public static string FixedHref(this UrlHelper urlHelper, Func<string, string> href, string url) {
	        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
	            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) {
	            return url;
	        }
	        url = String.IsNullOrWhiteSpace(url) ? "/" : url;
	        return href(Uri.IsWellFormedUriString(url, UriKind.Relative) ? url : "~/invalid-url");
	    }

	    public static IDictionary<string, string> Set(this IDictionary<string, string> attributes, string key, string value) {
			if (attributes.ContainsKey(key)) {
				attributes[key] = value;
			}
			else {
				attributes.Add(key, value);
			}
			return attributes;
		}

		public static IDictionary<string, string> MergeStyles(this IDictionary<string, string> attributes, IDictionary<string, string> styles) {
			if (attributes.ContainsKey("style")) {
				var styleString = attributes["style"];

				if (styleString != null) {
					var currentStyles = ParseStyleString(styleString);

					foreach (var item in styles) {
						if (currentStyles.ContainsKey(item.Key)) {
							currentStyles[item.Key] = item.Value;
						}
						else {
							currentStyles.Add(item.Key, item.Value);
						}
					}

				}
			}
			else {
				attributes.Add("style", BuildStyle(styles));
			}

			return attributes;
		}

		public static void AddIfNotEmpty(this IDictionary<string, string> styles, string key, string value) {
			if (!String.IsNullOrWhiteSpace(value)) {
				styles.Add(key, value);
			}
		}

		public static void AddIfNonZero(this IDictionary<string, string> styles, string key, int value) {
			if (value != 0) {
				styles.Add(key, value.ToString(CultureInfo.InvariantCulture) + "px");
			}
		}

		public static void AddPercentIfNonZeroPixels(this IDictionary<string, string> styles,
			string key, Length length, int reference,
			string extraAttributeName = null, string extraAttributeValue = null) {

			if (length.Value <= 0) return;
			styles.Add(key, Percentage(length, reference));
			if (extraAttributeName != null &&
				extraAttributeValue != null &&
				!styles.ContainsKey(extraAttributeName)) {

				styles.Add(extraAttributeName, extraAttributeValue);
			}
		}

		public static string Percentage(Length absolute, int reference) {
			if (reference == 0 || absolute.Unit != Length.Pixel) return absolute.ToString();
			return (100*absolute.Value/reference).ToString("f", CultureInfo.InvariantCulture) + "%";
		}

		public static int GetIndex(HttpContextBase context, string name, int startAt = 1, bool increment = true) {
			var rawValue = context.Items[name];
			var value = rawValue != null ? (int)rawValue : startAt - 1;
			if (increment) {
				value++;
				context.Items[name] = value;
			}
			return value;
		}

		public static int GetIndex(this HtmlHelper html, string name, int startAt = 1, bool increment = true) {
			return GetIndex(html.ViewContext.HttpContext, name, startAt, increment);
		}

		public static void ResetIndex(HttpContextBase context, string name) {
			context.Items.Remove(name);
		}

		public static void ResetIndex(this HtmlHelper html, string name) {
			ResetIndex(html.ViewContext.HttpContext, name);
		}

		public static int GetLayoutEditorHelperIndex(this HtmlHelper html, bool increment = true) {
			return html.GetIndex("LayoutEditorHelperIndex", increment: increment);
		}

		public static void ResetLayoutEditorHelperIndex(this HtmlHelper html) {
			ResetIndex(html.ViewContext.HttpContext, "LayoutEditorHelperIndex");
		}

		public static int GetLayoutEditorIndex(this HtmlHelper html, bool increment = true) {
			return html.GetIndex("LayoutEditorIndex", 0, increment);
		}

		public static int GetLayoutDisplayIndex(this HtmlHelper html, bool increment = true) {
			return html.GetIndex("LayoutDisplayIndex", 0, increment);
		}

		private static IDictionary<string, string> ParseStyleString(string style) {
			return style.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(x => x.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
						.ToDictionary(x => x[0], x => x[1]);
		}
	}
}
