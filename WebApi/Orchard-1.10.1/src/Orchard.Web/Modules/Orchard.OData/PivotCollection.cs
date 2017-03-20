
namespace Orchard.OData
{
    using Orchard.ContentManagement;
    using Orchard.Core.Common.Models;
    using Orchard.Core.Title.Models;
    using Orchard.Fields.Fields;
    using Orchard.MediaLibrary.Fields;
    using Orchard.OData.Services;
    using PivotServerTools;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;

    public interface IODataPivotCollection : IDependency {
        Collection MakeCollection(HttpContext httpContext);
    }

    public sealed class PivotCollection : IODataPivotCollection
    {
        private readonly IODataServiceContext oDataServiceContext;
        public PivotCollection(IODataServiceContext oDataServiceContext) {
            this.oDataServiceContext = oDataServiceContext;
        }

        Collection IODataPivotCollection.MakeCollection(HttpContext httpContext)
        {
            var requestUri = httpContext.Request.Url;
            var fragments = requestUri.AbsoluteUri.Split('/').AsEnumerable();
            var lastFragment = fragments.Last();

            var contentTypeName = Path.GetFileNameWithoutExtension(lastFragment);

            var orchardHostUrl = requestUri.AbsoluteUri.Substring(0, requestUri.AbsoluteUri.IndexOf("/odata"));
            var odataServiceUrl = string.Format("{0}/{1}", orchardHostUrl, "odata");
            var odataContentTypeUrl = string.Format("{0}/{1}s", odataServiceUrl, contentTypeName);

            this.oDataServiceContext.CustomState = this.oDataServiceContext.GetAnnotation(new Uri(odataServiceUrl), new Uri(odataContentTypeUrl), null);

            var coll = new Collection();
            coll.Name = contentTypeName;
            coll.EnableInfoPaneBingSearch = false;
            coll.IconUrl = new Uri(string.Format("{0}/{1}", odataServiceUrl, "images/OrchardLogo.png"));

            var queryable = this.oDataServiceContext.Query(coll.Name) as IQueryable<IContent>;
            foreach (var contentItem in queryable.ToList())
            {
                var iconUrl = coll.IconUrl;
                var id = contentItem.Id;
                var title = contentItem.Has<TitlePart>() ? contentItem.As<TitlePart>().Title : id.ToString();

                var metadata = contentItem.ContentItem.ContentManager.GetItemMetadata(contentItem);
                var relative = string.Join("/", metadata.DisplayRouteValues.Values);
                var url = string.Format("{0}/{1}", orchardHostUrl, relative);

                var facets = new List<Facet>() { };
                var commonPart = contentItem.As<CommonPart>();
                var facetPublished = new Facet("PublishedUtc", commonPart.PublishedUtc.Value); // All oData items are published ones
                facets.Add(facetPublished);

                var fieldPart = contentItem.ContentItem.Parts.FirstOrDefault(part => part.PartDefinition.Name.ToLower() == contentTypeName.ToLower());
                if (null != fieldPart) 
                {
                    var mediaPickerField = fieldPart.Fields.FirstOrDefault(field => field is MediaLibraryPickerField) as MediaLibraryPickerField;
                    if (null != mediaPickerField)
                    {
                        var mediaPart = mediaPickerField.MediaParts.FirstOrDefault();
                        if (null != mediaPart)
                        {
                            iconUrl = new Uri(mediaPart.MediaUrl);
                        }
                    }

                    var dateTimeFacets = fieldPart
                        .Fields
                        .Where(contentField => contentField is DateTimeField)
                        .Select(contentField => contentField as DateTimeField)
                        .Select(contentField => new Facet(contentField.DisplayName, contentField.DateTime));
                    
                    var numericFacets = fieldPart
                        .Fields
                        .Where(contentField => contentField is NumericField)
                        .Select(contentField => contentField as NumericField)
                        .Select(contentField => new Facet(contentField.DisplayName, (double)(contentField.Value.HasValue ? contentField.Value.Value : 0)));

                    var linkFacets = fieldPart
                        .Fields
                        .Where(contentField => contentField is LinkField)
                        .Select(contentField => contentField as LinkField)
                        .Select(contentField => new Facet(contentField.DisplayName, new FacetHyperlink(contentField.Name, contentField.Target)));

                    var inputFacets = fieldPart
                        .Fields
                        .Where(contentField => contentField is InputField)
                        .Select(contentField => contentField as InputField)
                        .Select(contentField => new Facet(contentField.DisplayName, contentField.Value));

                    var enumerationFacets = fieldPart
                        .Fields
                        .Where(contentField => contentField is EnumerationField)
                        .Select(contentField => contentField as EnumerationField)
                        .Select(contentField => new Facet(contentField.DisplayName, contentField.Value));

                    var booleanFacets = fieldPart
                        .Fields
                        .Where(contentField => contentField is BooleanField)
                        .Select(contentField => contentField as BooleanField)
                        .Select(contentField => new Facet(contentField.DisplayName, contentField.Value.GetValueOrDefault() ? 1 : 0));

                    facets.AddRange(numericFacets);
                    facets.AddRange(booleanFacets);
                    facets.AddRange(dateTimeFacets);
                    facets.AddRange(inputFacets);
                    facets.AddRange(enumerationFacets);
                    facets.AddRange(linkFacets);
                }

                coll.AddItem(title, url, string.Empty, new ItemImage(iconUrl), facets.ToArray());
            }

            return coll;
        }
    }
}