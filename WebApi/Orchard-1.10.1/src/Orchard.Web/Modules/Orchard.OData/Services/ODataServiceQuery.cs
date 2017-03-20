
namespace Orchard.OData.Services
{
    using Microsoft.Practices.DataServiceProvider;
    using Orchard.ContentManagement;
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IODataServiceQuery : IDependency, IDataServiceQueryProvider
    {
    }

    public sealed class ODataServiceQuery : IODataServiceQuery
    {
        private readonly IODataServiceMetadata _metadata;
        private readonly IODataServiceContext _queryContext;
        public ODataServiceQuery(
            IODataServiceMetadata metadata,
            IODataServiceContext queryContext)
        {
            this._metadata = metadata;
            this._queryContext = queryContext;
        }

        bool IDataServiceQueryProvider.IsNullPropagationRequired
        {
            // Our provider requires null propagation because it relies on LINQ to Objects. LINQ to Objects simply compiles
            // the expression into IL and executes it. So if there's an access to a member on instance which is null it will throw NullReferenceException
            // So we need the null checks in the expression tree to avoid this situation.
            get { return true; }
        }

        object IDataServiceQueryProvider.CurrentDataSource
        {
            get { return this._queryContext; }
            set { return; }
        }

        object IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
        {
            throw new NotImplementedException();
        }

        IEnumerable<KeyValuePair<string, object>> IDataServiceQueryProvider.GetOpenPropertyValues(object target)
        {
            throw new NotImplementedException();
        }

        object IDataServiceQueryProvider.GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            if (target is ContentItem)
            {
                var contentItem = (ContentItem)target;
                var contentPart =  contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == resourceProperty.Name);
                if (null != contentPart) return contentPart;
                var resourceField = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == contentItem.ContentType);
                if (null == resourceField) return null;
                var contentField = resourceField.Fields.FirstOrDefault(x => x.Name == resourceProperty.Name);
                return contentField;
            }
            if (target is ContentPart)
            {
                var partProperty = resourceProperty.GetAnnotation().InstanceProperty;
                // ContentPartRecord POCO accessor
                if (null != partProperty) { 
                    var partPropertyValue = partProperty.GetValue(target, null);
                    return partPropertyValue;
                }
                //ContentField accessor
                var contentPart = target as ContentPart;
                var contentField = contentPart.Fields.FirstOrDefault(x => x.Name == resourceProperty.ResourceType.Name);
                return contentField;
            }
            if (target is ContentField)
            {
                var fieldProperty = resourceProperty.GetAnnotation().InstanceProperty;
                var fieldPropertyvalue = fieldProperty.GetValue(target, null);
                return fieldPropertyvalue;
            }
            return null;
        }

        IQueryable IDataServiceQueryProvider.GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            var contenTypeDefinition = ((ResourceType)resourceSet.CustomState).Name;
            var underlyingQueryable = this._queryContext.Query(contenTypeDefinition);
            var visitor = new ODataServiceInterceptedQueryVisitor(this as IDataServiceQueryProvider);
            return ODataServiceInterceptedQueryProvider.CreateQuery(underlyingQueryable, visitor);
        }

        ResourceType IDataServiceQueryProvider.GetResourceType(object target)
        {
            ResourceType resourceType = null;
            var name = string.Empty;

            if (target is ContentItem) {
                name = ((ContentItem)target).TypeDefinition.Name;
            }
            if (target is ContentPart) {
                name = ((ContentPart)target).PartDefinition.Name;
            }
            if (target is ContentField){
                name = ((ContentField)target).Name;
            }

            (this._metadata as IDataServiceMetadataProvider).TryResolveResourceType(name, out resourceType); 
            return resourceType;
        }

        object IDataServiceQueryProvider.InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}