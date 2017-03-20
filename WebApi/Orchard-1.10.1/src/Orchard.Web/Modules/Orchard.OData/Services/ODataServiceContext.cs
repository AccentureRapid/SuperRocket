
namespace Orchard.OData.Services
{
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Core.Contents.Settings;
    using Orchard.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IODataServiceContext : IDependency
    {
        object CustomState { get; set; }
        string Expand { get; set; }
        string Filter { get; set; }
        string OrderBy { get; set; }
        int FirstResults { get; set; }
        int MaxResults { get; set; }
        IEnumerable<ContentTypeDefinition> Metadata { get; }
        ContentItem New(string contentTypeDefinition);
        IQueryable Query(string contentTypeDefinition);
    }

    public sealed class ODataServiceContext : IODataServiceContext, IQueryProvider, IOrderedQueryable<ContentItem>
    {
        private List<string> _includes = new List<string>();
        private string _filterClause = string.Empty;
        private string _orderByClause = string.Empty;
        private Expression _expression = Expression.Empty();
        private IContentQuery<ContentItem> _contentQuery = null;
        private IEnumerable<ContentItem> _queryResults = null;
        private object _customState = null;
        private int _maxResults = 0;
        private int _firstResults = 0;

        private readonly IContentManager _contentManager;
        private readonly ISessionLocator _sessionLocator;
        public ODataServiceContext(
            ISessionLocator sessionLocator,
            IContentManager contentManager)
        {
            this._contentManager = contentManager;
            this._sessionLocator = sessionLocator;
        }

        ContentItem IODataServiceContext.New(string contentTypeDefinition)
        {
            return this._contentManager.New(contentTypeDefinition);
        }

        IQueryable IODataServiceContext.Query(string contentTypeDefinition)
        {
            this._contentQuery = this._contentManager
                .Query()
                .ForType(new string[] { contentTypeDefinition })
                .ForVersion(VersionOptions.Published);

            if (this._includes.Any())
            {
                var contentItem = this._contentManager.New(contentTypeDefinition);

                var contentPartTypes = contentItem
                    .Parts
                    .Where(part => this._includes.Contains(part.PartDefinition.Name))
                    .Select(part => part.GetType());

                var expandPartsMethod = typeof(QueryHints)
                    .GetMethods()
                    .FirstOrDefault(m => m.IsGenericMethod && m.GetGenericArguments().Count() == contentPartTypes.Count() && m.Name == "ExpandParts");

                if (null != expandPartsMethod)
                {
                    expandPartsMethod = expandPartsMethod.MakeGenericMethod(contentPartTypes.ToArray());
                    var queryHints = (QueryHints)expandPartsMethod.Invoke(new QueryHints(), new object[] { });
                    this._contentQuery.WithQueryHints(queryHints);
                }
            }

            var memberExpression = MemberExpression.Property(MemberExpression.Constant(this), typeof(ODataServiceContext), "Query");
            return (this as IQueryProvider).CreateQuery<ContentItem>(memberExpression);
        }

        IEnumerable<ContentTypeDefinition> IODataServiceContext.Metadata
        {
            get
            {
                //return this._contentManager.GetContentTypeDefinitions().Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Creatable);
                return this._contentManager.GetContentTypeDefinitions().Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Listable);
            }
        }

        object IODataServiceContext.CustomState
        {
            get { return this._customState; }
            set { this._customState = value; }
        }

        string IODataServiceContext.Expand
        {
            get 
            {
                return string.Join(",", this._includes);
            }
            set 
            {
                if (string.IsNullOrEmpty(value)) return;

                IEnumerable<string> expand = value
                    .Split(',')
                    .Select(s => s.Substring(s.IndexOf('/') + 1).Replace('/', '.'))
                    .AsEnumerable();
                if (!expand.Any()) return;

                this._includes.Clear();
                expand.ToList().ForEach(this._includes.Add);
            }
        }

        string IODataServiceContext.Filter
        {
            get { return this._filterClause; }
            set { this._filterClause = value; }
        }

        string IODataServiceContext.OrderBy
        {
            get { return this._orderByClause ; }
            set { this._orderByClause = value; }
        }

        int IODataServiceContext.FirstResults
        {
            get { return this._firstResults; }
            set { this._firstResults = value; }
        }

        int IODataServiceContext.MaxResults
        {
            get { return this._maxResults; }
            set { this._maxResults = value; }
        }

        public IQueryable<ContentItem> Query
        {
            get { return this._queryResults.AsQueryable() ?? new List<ContentItem>() { }.AsQueryable(); }
        }

        #region [ == IQueryProvider == ]

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(ContentItem)) {
                this._queryResults = this._contentQuery.List();
                var lambda = Expression.Lambda<Func<IQueryable<TElement>>>(expression).Compile();
                return lambda();
            }
            this._expression = expression;
            return (IQueryable<TElement>)((this as IOrderedQueryable<ContentItem>).AsQueryable());
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return (this as IQueryProvider).CreateQuery<ContentItem>(expression);
        }
        
        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            this._expression = expression;

            try
            {
                // == Here apply expression onto full set of already retrieved data while expression as been inspected with InterceptedQueryVisitor
                this._queryResults = this._contentQuery.List();
                var lambda = Expression.Lambda<Func<IQueryable<ContentItem>>>(this._expression).Compile();
                this._queryResults = lambda().AsEnumerable();

                // == Here prefilter query before been applied on repository
                //var contentItemRecordIds = this.FilterAndOrderContentQuery(this._sessionLocator.For(typeof(ContentItem)));
                //this._queryResults = this._contentManager.GetMany<ContentItem>(contentItemRecordIds, VersionOptions.Published, QueryHints.Empty);
                //this._queryResults = this._queryResults.OrderBy(ci => ci.VersionRecord.Id, new OrderedContentItemIdComparer(contentItemRecordIds.ToList()));

                //this._queryResults = this._queryResults.Select(CloneAndExpandContent); 
            }
            catch {
                this._queryResults = new List<ContentItem>() { };
            }

            return (TResult)this._queryResults;            
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return (this as IQueryProvider).Execute<IEnumerable<ContentItem>>(expression);
        }

        #endregion

        #region [ == IQueryable == ]

        Type IQueryable.ElementType
        {
            get { return typeof(ContentItem); }
        }

        Expression IQueryable.Expression
        {
            get { return this._expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return this; }
        }

        IEnumerator<ContentItem> IEnumerable<ContentItem>.GetEnumerator()
        {
            var results = (this as IQueryProvider).Execute(this._expression) as IEnumerable<ContentItem>;
            return results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<ContentItem>).GetEnumerator();
        }

        #endregion

        private ContentItem CloneAndExpandContent(ContentItem contentItem)
        {
            if (!this._includes.Any())
            {
                return contentItem;
            }

            var clonedContentItem = this._contentManager.New(contentItem.ContentType);

            // Add the contentPart that deals with fields
            var partsNameToClone = this._includes.Concat(new List<string>() { contentItem.ContentType });
            var partsToClone = contentItem.Parts.Where(part => partsNameToClone.Contains(part.PartDefinition.Name));

            foreach (var referencePart in partsToClone)
            {
                var clonedPart = clonedContentItem.Parts.FirstOrDefault(p => p.PartDefinition.Name == referencePart.PartDefinition.Name);

                referencePart
                    .GetType()
                    .GetProperties()
                    .Where(p => p.GetSetMethod() != null && p.GetGetMethod() != null)
                    .ToList()
                    .ForEach(p => p.SetValue(clonedPart, p.GetValue(referencePart), null));

                var p_fields = typeof(ContentPart).GetField("_fields", BindingFlags.NonPublic | BindingFlags.Instance);
                (p_fields.GetValue(clonedPart) as IList<ContentField>).Clear();
                referencePart.Fields.ToList().ForEach(f => clonedPart.Weld(f));
            }

            return clonedContentItem;
        }
    }

    internal class OrderedContentItemIdComparer : IComparer<int>
    {
        private readonly IList<int> orderedIds;
        internal OrderedContentItemIdComparer(IList<int> orderedIds)
        {
            this.orderedIds = orderedIds;
        }

        int IComparer<int>.Compare(int x, int y)
        {
            var indexOfX = this.orderedIds.IndexOf(x);
            var indexOfY = this.orderedIds.IndexOf(y);
            return indexOfX.CompareTo(indexOfY);
        }
    }
}