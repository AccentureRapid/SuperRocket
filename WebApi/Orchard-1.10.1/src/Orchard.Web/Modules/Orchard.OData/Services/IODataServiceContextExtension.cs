

namespace Orchard.OData.Services
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Csdl;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using NHibernate;
    using NHibernate.Criterion;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.Records;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    internal static class IODataServiceContextExtension
    {
        private class ODataParser
        {
            public IEdmModel EdmModel { get; set; }
            public IEdmEntitySet EdmEntitySet { get; set; }
            public IEdmEntityType EdmEntityType { get; set; }
            public ODataUriParser InternalParser { get; set; }
            public IODataServiceMetadata Metadata { get; set; }
        }

        internal static object GetAnnotation(this IODataServiceContext context, params object[] customs)
        {
            if (false == (null != customs && customs.Count() <= 3)) {
                return null;
            }

            var serviceUri = customs[0] as Uri;
            var requestUri = customs[1] as Uri;
            var metadata = customs[2] as IODataServiceMetadata;

            IEdmModel edmModel = EdmxReader.Parse(XmlReader.Create(serviceUri + "/$metadata"));
            IEdmEntitySet edmEntitySet = edmModel.EntityContainers()
                .First()
                .EntitySets()
                .FirstOrDefault(es => 
                    es.Name.ToLower() == requestUri.Segments.Last().ToLower() ||
                    requestUri.Segments.Last().ToLower().StartsWith(es.Name.ToLower() + "(")
                );
            IEdmEntityType edmEntityType = edmEntitySet.ElementType;
            ODataUriParser internalParser = new ODataUriParser(edmModel, serviceUri);

            var odataParser = new ODataParser() {
                EdmModel = edmModel,
                EdmEntitySet = edmEntitySet,
                EdmEntityType = edmEntityType,
                InternalParser = internalParser,
                Metadata = metadata,
            };

            return odataParser;
        }

        internal static IEnumerable<int> FilterAndOrderContentQuery(this IODataServiceContext context, ISession session)
        {
            var odataParser = context.CustomState as ODataParser;
            var ids = new List<int>() {} as IEnumerable<int>;

            if (null == odataParser) {
                return ids;
            }

            var contentItemVersionRecordCriteria = session.CreateCriteria<ContentItemVersionRecord>();
            contentItemVersionRecordCriteria.Add(Restrictions.Eq("Published", true));

            string contentTypeName = odataParser.EdmEntityType.Name;
            var contentItemRecordCriteria = contentItemVersionRecordCriteria.BindCriteriaByPath("ContentItemRecord");
            var contentTypeCriteria = contentItemRecordCriteria.BindCriteriaByPath("ContentType");
            contentTypeCriteria.Add(Restrictions.Eq("Name", contentTypeName));

            if (null != odataParser.Metadata && !string.IsNullOrEmpty(context.Filter)) {
                var filterClause = odataParser.InternalParser.ParseFilter(context.Filter, odataParser.EdmEntityType, odataParser.EdmEntitySet);
                contentItemVersionRecordCriteria = contentItemVersionRecordCriteria.FilterContentQuery(filterClause, odataParser.Metadata);
            }

            if (null != odataParser.Metadata && !string.IsNullOrEmpty(context.OrderBy)) {
                var orderByClause = odataParser.InternalParser.ParseOrderBy(context.OrderBy, odataParser.EdmEntityType, odataParser.EdmEntitySet);
                contentItemVersionRecordCriteria = contentItemVersionRecordCriteria.OrderContentQuery(orderByClause, odataParser.Metadata);
            }

            if (0 < context.FirstResults) {
                contentItemVersionRecordCriteria.SetFirstResult(context.FirstResults);
            }

            if (0 < context.MaxResults) {
                contentItemVersionRecordCriteria.SetMaxResults(context.MaxResults);
            }

            var hQueryResults = contentItemVersionRecordCriteria.List();
            if (1 > hQueryResults.Count) {
                return ids;
            }

            ids = hQueryResults
                .Cast<ContentItemVersionRecord>()
                .Select(civr => civr.ContentItemRecord.Id)
                .Distinct();
            return ids;
        }
    }
}