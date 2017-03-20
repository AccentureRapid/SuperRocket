// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft" file="ResourcePropertyAnnotation.cs">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Microsoft Public License.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
// </copyright>
// <summary>
//   Helper class for extension methods on the <see cref="ResourceProperty" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Practices.DataServiceProvider
{
    using System.Data.Services.Providers;
    using System.Reflection;

    /// <summary>
    /// Helper class for extension methods on the <see cref="ResourceProperty"/>.
    /// </summary>
    internal static class ResourcePropertyExtensions
    {
        /// <summary>
        /// Helper method to get annotation from the specified resource property.
        /// </summary>
        /// <param name="resourceProperty">
        /// The resource Property.
        /// </param>
        /// <returns>
        /// The annotation for the resource property or null if the resource property doesn't have annotation.
        /// </returns>
        /// <remarks>
        /// We store the annotation in the <see cref="ResourceProperty.CustomState"/>, so this is just a simple helper
        /// which allows strongly typed access.
        /// </remarks>
        internal static ResourcePropertyAnnotation GetAnnotation(this ResourceProperty resourceProperty)
        {
            if (null == resourceProperty.CustomState)
            {
                resourceProperty.CustomState = new ResourcePropertyAnnotation();
            }

            return resourceProperty.CustomState as ResourcePropertyAnnotation;
        }
    }

    /// <summary>
    /// Class used to annotate <see cref="ResourceProperty"/> instances with DSP specific data.
    /// </summary>
    internal class ResourcePropertyAnnotation
    {
        /// <summary>Gets or sets The CLR property which backs this resource property.</summary>
        public PropertyInfo InstanceProperty { get; set; }

        /// <summary>Gets or sets the resource association set which describes the reference this annotation stores.</summary>
        public ResourceAssociationSet ResourceAssociationSet { get; set; }
    }
}