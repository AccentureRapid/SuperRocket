// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Microsoft Public License.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.OData.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class ODataServiceInterceptedQueryVisitor : ExpressionVisitor
    {
        private readonly Expression<Func<object, ResourceProperty, object>> _getValueReplacement;
        private readonly Expression<Func<object, ResourceType, bool>> _typeIsReplacement;
        private readonly IDataServiceQueryProvider _dataServiceQueryProvider;
        public ODataServiceInterceptedQueryVisitor(IDataServiceQueryProvider dataServiceQueryProvider)
        {
            this._dataServiceQueryProvider = dataServiceQueryProvider;
            this._getValueReplacement = (o, rp) => this._dataServiceQueryProvider.GetPropertyValue(o, rp);
            this._typeIsReplacement = (o, rp) => this._dataServiceQueryProvider.GetResourceType(o).Name == rp.Name;
        }

        static readonly MethodInfo GetValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
                "GetValue",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(object), typeof(ResourceProperty) },
                null);

        static readonly MethodInfo GetSequenceValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
                "GetSequenceValue",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(object), typeof(ResourceProperty) },
                null);

        static readonly MethodInfo ConvertMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
               "Convert",
               BindingFlags.Static | BindingFlags.Public);

        static readonly MethodInfo TypeIsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
                "TypeIs",
                BindingFlags.Static | BindingFlags.Public);

        static readonly MethodInfo CompareMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
                "Compare",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type [] { typeof(string), typeof(string) },
                null);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == GetValueMethodInfo)
            {
                // Arguments[0] - the resource to get property from 
                // Arguments[1] - the ResourceProperty to get 
                // Invoke the replacement expression, passing the 
                // appropriate parameters. 
                return Expression.Invoke(
                   Expression.Quote(this._getValueReplacement),
                   this.Visit(node.Arguments[0]),
                   node.Arguments[1]);
            }
            else if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == GetSequenceValueMethodInfo)
            {
                // Arguments[0] - the resource  
                // Arguments[1] - the Property that is a sequence  
                // Just call the GetValueReplacement(0,1) and 
                // cast it to IEnumerable<T> which is the  
                // correct return type 
                return Expression.Convert(
                    Expression.Invoke(
                        Expression.Quote(this._getValueReplacement),
                        this.Visit(node.Arguments[0]),
                        node.Arguments[1]),
                    node.Method.ReturnType);
            }
            else if (node.Method == TypeIsMethodInfo)
            {
                // Arguments[0] – the resource 
                // Arguments[1] – the ResourceType 
                // Invoke the TypeIsReplacement expression 
                // binding to the resource & resourceType 
                return Expression.Invoke(
                   Expression.Quote(this._typeIsReplacement),
                   this.Visit(node.Arguments[0]),
                   node.Arguments[1]
                );
            }
            else if (node.Method == ConvertMethodInfo)
            {
                // Arguments[0] – the resource 
                // Arguments[0] – the ResourceType 
                // no need to do anything, so just 
                // return the argument 
                return this.Visit(node.Arguments[0]);
            }

            return base.VisitMethodCall(node);
        }
    }
}