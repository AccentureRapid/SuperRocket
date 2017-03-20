
namespace Orchard.OData.Services
{
    using Microsoft.Practices.DataServiceProvider;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.MetaData.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Linq;
    using System.Reflection;

    public interface IODataServiceMetadata : IDependency, IDataServiceMetadataProvider
    {
    }

    public sealed class ODataServiceMetadata : IODataServiceMetadata
    {
        private readonly Dictionary<ResourceType, ResourceSet> _resourceSets = new Dictionary<ResourceType, ResourceSet>();
        private readonly Dictionary<string, ResourceType> _resourceTypes = new Dictionary<string, ResourceType>();
        private readonly IODataServiceContext _queryEntityService;
        public ODataServiceMetadata(IODataServiceContext queryEntityService)
        {
            this._queryEntityService = queryEntityService;
            this.InitializeResources();
        }

        string IDataServiceMetadataProvider.ContainerName
        {
            get 
            {
                var namespaces = (this as IDataServiceMetadataProvider).ContainerNamespace.Split('.').AsEnumerable();
                return string.Join("", namespaces);
            }
        }

        string IDataServiceMetadataProvider.ContainerNamespace
        {
            get 
            {
                var namespaces = typeof(ODataServiceMetadata).FullName.Split('.').AsEnumerable();
                namespaces = namespaces.Except(new string[] { namespaces.Last() });
                namespaces = namespaces.Except(new string[] { namespaces.Last() });
                return string.Join(".", namespaces);
            }
        }

        IEnumerable<ResourceType> IDataServiceMetadataProvider.Types
        {
            get { return this._resourceTypes.Select(kvp => kvp.Value); }
        }

        IEnumerable<ResourceSet> IDataServiceMetadataProvider.ResourceSets
        {
            get { return this._resourceSets.Select(kvp => kvp.Value); }
        }

        IEnumerable<ServiceOperation> IDataServiceMetadataProvider.ServiceOperations
        {
            // No service operations yet 
            get { yield break; }
        }

        IEnumerable<ResourceType> IDataServiceMetadataProvider.GetDerivedTypes(ResourceType resourceType)
        {
            // No inheritance accepted
            yield break; 
        }

        ResourceAssociationSet IDataServiceMetadataProvider.GetResourceAssociationSet(
            ResourceSet resourceSet,
            ResourceType resourceType,
            ResourceProperty resourceProperty)
        {
            // We have the resource association set precreated on the property annotation,
            // so no need to compute anything in here
            ResourceAssociationSet resourceAssociationSet = resourceProperty.GetAnnotation().ResourceAssociationSet;
            return resourceAssociationSet;
        }

        bool IDataServiceMetadataProvider.HasDerivedTypes(ResourceType resourceType)
        {
            return false;
        }

        bool IDataServiceMetadataProvider.TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            resourceSet = null;

            var keyValuePair = this._resourceSets.FirstOrDefault(kvp => kvp.Value.Name.ToLower() == name.ToLower());
            var foundResourceType = keyValuePair.Key;
            if (foundResourceType == null) {
                return false;
            }

            resourceSet = foundResourceType.GetAnnotation().ResourceSet;
            if (resourceSet == null) {
                return false;
            }

            return true;
        }

        bool IDataServiceMetadataProvider.TryResolveResourceType(string name, out ResourceType resourceType)
        {
            resourceType = null;
            if (this._resourceTypes.ContainsKey(name)) resourceType = this._resourceTypes[name];
            return null != resourceType;
        }

        bool IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            serviceOperation = null;
            return false;
        }

        private void InitializeResources()
        {
            // Create resource types
            this._queryEntityService.Metadata.ToList().ForEach(this.CreateResourceType);

            // Set resource types and resource sets to read only
            this._resourceSets.Select(kvp => kvp.Value).ToList().ForEach(rs => rs.SetReadOnly());
            this._resourceTypes.Select(kvp => kvp.Value).ToList().ForEach(rt => rt.SetReadOnly());
        }

        private void CreateResourceType(ContentTypeDefinition contentTypeDefinition)
        {
            // Try to get the resource type from the dictionary
            if (this._resourceTypes.ContainsKey(contentTypeDefinition.Name)) {
                return;
            }

            // Create resource type
            var resourceTypeContentType = new ResourceType(
                typeof(ContentItem),
                ResourceTypeKind.EntityType, 
                null, 
                (this as IDataServiceMetadataProvider).ContainerNamespace, 
                contentTypeDefinition.Name, 
                false);
            resourceTypeContentType.CanReflectOnInstanceType = true;

            var resourceProperty = new ResourceProperty(
                "Id", 
                ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, 
                ResourceType.GetPrimitiveResourceType(typeof(int)));
            resourceProperty.CanReflectOnInstanceTypeProperty = true;
            resourceTypeContentType.AddProperty(resourceProperty);

            // Add the resource type to the dictionary
            this._resourceTypes[contentTypeDefinition.Name] = resourceTypeContentType;

            var contentItem = this._queryEntityService.New(contentTypeDefinition.Name);
            contentItem.Parts
                .Join(
                    inner: contentItem.TypeDefinition.Parts, 
                    outerKeySelector: contentPart => contentPart.PartDefinition.Name,
                    innerKeySelector: contentypePartDefinition => contentypePartDefinition.PartDefinition.Name,
                    resultSelector: (contentPart, partDefinition) => contentPart)
                .ToList()
                .ForEach(contentPart => this.CreateResourceProperty(resourceTypeContentType, contentPart));

            // Provide ResourceSet
            ResourceSet resourceSet = new ResourceSet(resourceTypeContentType.Name + "s", resourceTypeContentType);
            resourceTypeContentType.GetAnnotation().ResourceSet = resourceSet;
            // Add annotation to resource set so we can request on this ResourceType
            resourceSet.CustomState = resourceTypeContentType;

            this._resourceSets[resourceTypeContentType] = resourceSet;
        }

        private void CreateResourceProperty(ResourceType resourceType, ContentPart contentPart)
        {
            if (resourceType.Name == contentPart.PartDefinition.Name)
            {
                contentPart.Fields.ToList().ForEach(contentField => {
                    var resourceTypeContentField = this.GetOrCreateContentField(contentField);
                    resourceType.AddResourcePropertyFromComplexType(resourceTypeContentField);
                });
                return;
            }

            var resourceTypeContentPart = this.GetOrCreateContentPart(contentPart);
            if (null != resourceTypeContentPart) {
                resourceType.AddResourcePropertyFromComplexType(resourceTypeContentPart);
            }
        }

        private ResourceType GetOrCreateContentPart(ContentPart contentPart)
        {
            var contentPartName = contentPart.PartDefinition.Name;
            var contentParType = contentPart.GetType();

            if (this._resourceTypes.ContainsKey(contentPartName)) {
                return this._resourceTypes[contentPartName];
            }

            var resourceTypeContentPart = this.GetOrCreateContentPart(contentParType, contentPartName);
            contentPart.Fields.ToList().ForEach(contentField =>
                {
                    var resourceTypeContentField = this.GetOrCreateContentField(contentField);
                    resourceTypeContentPart.AddResourcePropertyFromComplexType(resourceTypeContentField);
                });
            return resourceTypeContentPart;
        }

        private ResourceType GetOrCreateContentPart(Type contentPartType, string contentPartName)
        {
            if (this._resourceTypes.ContainsKey(contentPartName)) {
                return this._resourceTypes[contentPartName];
            }

            var resourceTypeContentPart = new ResourceType(
                typeof(ContentPart),
                ResourceTypeKind.ComplexType,
                null,
                (this as IDataServiceMetadataProvider).ContainerNamespace,
                contentPartName,
                false);
            resourceTypeContentPart.CanReflectOnInstanceType = false;
            resourceTypeContentPart.AddResourcePropertiesFromInstanceType(contentPartType);

            this._resourceTypes[contentPartName] = resourceTypeContentPart;
            return resourceTypeContentPart;
        }

        private ResourceType GetOrCreateContentField(ContentField contentField)
        {
            var contentFieldName = contentField.Name;
            if (this._resourceTypes.ContainsKey(contentFieldName)) {
                return this._resourceTypes[contentFieldName];
            }

            var contentFieldType = contentField.GetType();
            var resourceTypeContentField = new ResourceType(
                contentFieldType,
                ResourceTypeKind.ComplexType,
                null,
                (this as IDataServiceMetadataProvider).ContainerNamespace,
                contentFieldName,
                false);
            resourceTypeContentField.CanReflectOnInstanceType = false;
            resourceTypeContentField.AddResourcePropertiesFromInstanceType(contentFieldType);

            contentFieldType
                .GetProperties()
                .Where(property => property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .ToList()
                .ForEach(property =>
                {
                    var orchardType = property.PropertyType.GenericTypeArguments.Single();
                    if (orchardType.BaseType.IsGenericType && orchardType.BaseType.GetGenericTypeDefinition() == typeof(ContentPart<>))
                    {
                        var resourceType = this.GetOrCreateContentPart(orchardType, orchardType.Name);
                        resourceType.CustomState = property;
                        resourceTypeContentField.AddResourcePropertyFromInstanceCollectionResourceType(resourceType);
                    }
                });

            this._resourceTypes[contentFieldName] = resourceTypeContentField;
            return resourceTypeContentField;
        }
    }
}