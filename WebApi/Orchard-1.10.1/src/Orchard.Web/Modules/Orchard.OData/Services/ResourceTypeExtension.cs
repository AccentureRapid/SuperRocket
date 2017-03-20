
namespace Orchard.OData.Services
{
    using Microsoft.Practices.DataServiceProvider;
    using System;
    using System.Data.Services.Providers;
    using System.Linq;
    using System.Reflection;

    internal static class ResourceTypeExtension
    {
        internal static void AddResourcePropertiesFromInstanceType(this ResourceType resourceType, Type instanceType)
        {
            instanceType
                .GetProperties()
                .ToList()
                .ForEach(resourceType.AddResourcePropertyFromInstancePropertyInfo);
        }

        internal static void AddResourcePropertyFromInstancePropertyInfo(this ResourceType resourceType, PropertyInfo property)
        {
            var resourcePropertyType = ResourceType.GetPrimitiveResourceType(property.PropertyType);
            var innerEnumerableType = TypeSystem.GetIEnumerableElementType(property.PropertyType);
            var innerResourcePropertyType = null != innerEnumerableType ? ResourceType.GetPrimitiveResourceType(innerEnumerableType) : null;

            resourcePropertyType = null == resourcePropertyType && null != innerResourcePropertyType ? ResourceType.GetCollectionResourceType(innerResourcePropertyType) : resourcePropertyType;
            if (null == resourcePropertyType) {
                return;
            }

            var resourcePropertyKind = null == innerResourcePropertyType ? ResourcePropertyKind.Primitive : ResourcePropertyKind.Collection;
            var resourceProperty = new ResourceProperty(
                property.Name,
                resourcePropertyKind,
                resourcePropertyType);
            resourceProperty.CanReflectOnInstanceTypeProperty = false;
            resourceProperty.GetAnnotation().InstanceProperty = property;
            resourceType.AddProperty(resourceProperty);
        }

        internal static void AddResourcePropertyFromInstanceCollectionResourceType(this ResourceType resourceType, ResourceType propertyCollection)
        {
            var resourceProperty = new ResourceProperty(
                propertyCollection.Name,
                ResourcePropertyKind.Collection,
                ResourceType.GetCollectionResourceType(propertyCollection));
            resourceProperty.CanReflectOnInstanceTypeProperty = false;
            resourceProperty.GetAnnotation().InstanceProperty = (PropertyInfo)propertyCollection.CustomState;
            resourceType.AddProperty(resourceProperty);
        }

        internal static void AddResourcePropertyFromComplexType(this ResourceType resourceType, ResourceType resourcePropertyComplexType)
        {
            var resourceTypeProperty = new ResourceProperty(
                resourcePropertyComplexType.Name,
                ResourcePropertyKind.ComplexType,
                resourcePropertyComplexType);
            resourceTypeProperty.CanReflectOnInstanceTypeProperty = false;
            resourceType.AddProperty(resourceTypeProperty);
        }
    }
}