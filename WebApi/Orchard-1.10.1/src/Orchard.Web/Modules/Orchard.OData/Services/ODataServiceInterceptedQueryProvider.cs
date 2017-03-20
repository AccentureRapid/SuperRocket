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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class ODataServiceInterceptedQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _underlyingProvider;
        private ODataServiceInterceptedQueryVisitor _expressionVisitor;

        internal ODataServiceInterceptedQueryProvider(IQueryProvider underlyingQueryProvider, ODataServiceInterceptedQueryVisitor visitor)
        {
            this._underlyingProvider = underlyingQueryProvider;
            this._expressionVisitor = visitor;
        }

        public static IQueryable CreateQuery(IQueryable underlyingQuery, ExpressionVisitor visitor)
        {
            var provider = new ODataServiceInterceptedQueryProvider(underlyingQuery.Provider, (ODataServiceInterceptedQueryVisitor)visitor);
            return (provider as IQueryProvider).CreateQuery(underlyingQuery.Expression);
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new ODataServiceInterceptedQuery<TElement>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type et = TypeSystem.GetIEnumerableElementType(expression.Type);
            Type qt = typeof(ODataServiceInterceptedQuery<>).MakeGenericType(et);
            object[] args = new object[] { this, expression };

            ConstructorInfo ci = qt.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, 
                null, 
                new Type[] { typeof(ODataServiceInterceptedQueryProvider), typeof(Expression) }, 
                null);
            return (IQueryable)ci.Invoke(args);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            var processExpression = this.ProcessExpression(expression);
            return this._underlyingProvider.Execute<TResult>(processExpression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            var processExpression = this.ProcessExpression(expression);
            return this._underlyingProvider.Execute(processExpression);
        }

        internal IEnumerator<TElement> ExecuteQuery<TElement>(Expression expression)
        {
            var processExpression = this.ProcessExpression(expression);
            return _underlyingProvider
                .CreateQuery<TElement>(processExpression)
                .GetEnumerator();
        }

        private Expression ProcessExpression(Expression expression)
        {
            return this._expressionVisitor.Visit(expression);
        }
    }
}