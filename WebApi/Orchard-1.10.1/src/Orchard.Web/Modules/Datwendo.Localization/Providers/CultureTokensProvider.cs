using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Tokens;
using System.Globalization;

namespace Datwendo.Localization.Providers
{
    [OrchardFeature("Datwendo.Localization")]
    public class CultureTokensProvider : ITokenProvider
    {
        private readonly ICultureManager _cultureManager;

        public CultureTokensProvider(ICultureManager cultureManager)
        {
            T = NullLocalizer.Instance;
            _cultureManager = cultureManager;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context)
        {
            context.For("Content")
                .Token("CultureInfo", T("Culture"), T("Item's culture."));

            context.For("CultureInfo", T("Culture Information"), T("Tokens for Culture Informations"))
                .Token("CultureName", T("Culture name"), T("languagecode2-country/regioncode2."))
                .Token("ThreeLetterName", T("Three-letter name"), T("ISO 639-2 code for the language."))
                .Token("TwoLetterName", T("Two-letter name"), T("ISO 639-1 code for the language."));
        }

        public void Evaluate(EvaluateContext context)
        {
            context.For<IContent>("Content")
                .Token("Culture", content =>
                {
                    var ci = GetContentCultureInfo(content);
                    return ci == null ? string.Empty : ci.Name.ToLower();
                })
                .Chain("Culture", "CultureInfo", content => GetContentCultureInfo(content));

            context.For<CultureInfo>("CultureInfo")
                .Token("CultureName", ci => ci != null ? ci.Name.ToLower() : string.Empty)
                .Token("ThreeLetterName", ci => ci != null ? ci.ThreeLetterISOLanguageName : string.Empty)
                .Token("TwoLetterName", ci => ci != null ? ci.TwoLetterISOLanguageName : string.Empty);
        }

        private CultureInfo GetContentCultureInfo(IContent content)
        {
            var localizationPart = content != null ? content.As<LocalizationPart>() : null;
            if (localizationPart == null || localizationPart.Culture == null || string.IsNullOrEmpty(localizationPart.Culture.Culture)) return new CultureInfo(_cultureManager.GetSiteCulture());
            return new CultureInfo(localizationPart.Culture.Culture);
        }
    }

}
