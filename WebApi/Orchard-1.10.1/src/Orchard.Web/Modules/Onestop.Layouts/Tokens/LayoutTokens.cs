using System.Linq;
using System.Xml.Linq;
using Onestop.Layouts.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Onestop.Layouts.Tokens {
    [OrchardFeature("Onestop.Layouts")]
    public class LayoutTokens : ITokenProvider {
        private readonly IContentManager _contentManager;

        public LayoutTokens(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("Content", T("Content Items"), T("Content Items"))
                   .Token("TemplatedItem", T("TemplatedItem"), T("The templated item for this content item."));
            context.For("TemplatedItem", T("Templated Item"), T("Dynamically templated item"))
                   .Token("MainPicture", T("Main Picture"), T("The main picture for this templated item."));
        }

        public void Evaluate(EvaluateContext context) {
            context.For<IContent>("Content")
                   .Token("TemplatedItem", i => i == null ? "" : _contentManager.GetItemMetadata(i).DisplayText)
                   .Chain("TemplatedItem", "TemplatedItem", c => c.As<TemplatedItemPart>());
            context.For<TemplatedItemPart>("TemplatedItem")
                   .Token("MainPicture", part => {
                       var templated = part.As<TemplatedItemPart>();
                       if (templated != null) {
                           var data = templated.Data;
                           if (!string.IsNullOrWhiteSpace(data)) {
                               var dataDocument = XElement.Parse(data);
                               var mainPicture = dataDocument
                                   .Descendants()
                                   .FirstOrDefault(
                                       el =>
                                       (el.Name == "img" || el.Name == "container") &&
                                       el.Attribute("main") != null &&
                                       el.Attribute("main").Value == "true" &&
                                       ((el.Attribute("src") != null &&
                                         !string.IsNullOrWhiteSpace(el.Attribute("src").Value)) ||
                                        ((el.Attribute("background") != null &&
                                          !string.IsNullOrWhiteSpace(el.Attribute("background").Value)))));
                               if (mainPicture == null) {
                                   mainPicture = dataDocument
                                       .Descendants()
                                       .FirstOrDefault(
                                           el =>
                                           (el.Name == "img" || el.Name == "container") &&
                                           ((el.Attribute("src") != null &&
                                             !string.IsNullOrWhiteSpace(el.Attribute("src").Value)) ||
                                            ((el.Attribute("background") != null &&
                                              !string.IsNullOrWhiteSpace(el.Attribute("background").Value)))));
                               }
                               if (mainPicture != null) {
                                   return mainPicture.Name == "img"
                                              ? mainPicture.Attribute("src").Value
                                              : mainPicture.Attribute("background").Value;
                               }
                           }
                       }
                       return null;
                   });
        }
    }
}