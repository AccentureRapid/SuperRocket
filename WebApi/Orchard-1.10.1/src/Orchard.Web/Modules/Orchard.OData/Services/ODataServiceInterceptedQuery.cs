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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class ODataServiceInterceptedQuery<T> : IOrderedQueryable<T>
    {
        private Expression _expression;
        private ODataServiceInterceptedQueryProvider _provider;

        internal ODataServiceInterceptedQuery(ODataServiceInterceptedQueryProvider provider, Expression expression)
        {
            this._provider = provider;
            this._expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._provider.ExecuteQuery<T>(this._expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }
        Expression IQueryable.Expression
        {
            get { return this._expression; }
        }
        IQueryProvider IQueryable.Provider
        {
            get { return this._provider; }
        }
    }
}