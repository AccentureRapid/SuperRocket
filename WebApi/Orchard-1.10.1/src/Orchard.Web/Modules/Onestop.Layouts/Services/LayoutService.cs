using System.Collections.Generic;
using System.Linq;
using Onestop.Layouts.Elements;
using Onestop.Layouts.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Onestop.Layouts.Services {
    [OrchardFeature("Onestop.Layouts")]
    public class LayoutService : ILayoutService {
        private readonly IEnumerable<ILayoutElement> _layoutElements; 
        private readonly IContentManager _contentManager;

        public LayoutService(
            IEnumerable<ILayoutElement> layoutElements,
            IContentManager contentManager) {

            _layoutElements = layoutElements;
            _contentManager = contentManager;
        }

        public IEnumerable<LayoutTemplatePart> GetLayouts(ContentsOrder orderBy = ContentsOrder.Published) {
            var query = _contentManager
                .Query<LayoutTemplatePart>("OSLayout")
                .ForVersion(VersionOptions.Latest)
                .WithQueryHints(new QueryHints().ExpandParts<TitlePart>());

            switch (orderBy) {
                case ContentsOrder.Modified:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.ModifiedUtc);
                    break;
                case ContentsOrder.Published:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.PublishedUtc);
                    break;
                case ContentsOrder.Created:
                    query.OrderByDescending<CommonPartRecord>(cr => cr.CreatedUtc);
                    break;
            }
            return query.List();
        }

        public IEnumerable<dynamic> GetLayoutElementEditors() {
            return _layoutElements
                .SelectMany(
                    le => le.BuildLayoutEditors()
                )
                .OrderBy(el => el.Order);
        }
    }
}
