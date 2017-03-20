

namespace Orchard.OData.Services
{
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.SqlCommand;

    internal static class NHibernateCriteriaExtension
    {
        internal static ICriteria BindCriteriaByPath(this ICriteria criteria, string path)
        {
            return criteria.GetCriteriaByPath(path) ?? criteria.CreateCriteria(path);
        }

        internal static ICriteria BindCriteriaByAlias(this ICriteria criteria, string alias)
        {
            return criteria.GetCriteriaByAlias(alias);
        }

        internal static ICriteria BindCriteriaByAlias(this ICriteria criteria, string associationPath, string alias)
        {
            return criteria.BindCriteriaByAlias(alias) ?? criteria.CreateAlias(associationPath, alias).BindCriteriaByAlias(alias);
        }

        internal static ICriteria BindCriteriaByAlias(this ICriteria criteria, string associationPath, string alias, JoinType joinType)
        {
            return criteria.BindCriteriaByAlias(alias) ?? criteria.CreateAlias(associationPath, alias, joinType).BindCriteriaByAlias(alias);
        }

        internal static ICriteria FilterContentQuery(this ICriteria criteria, FilterClause filterClause, IODataServiceMetadata metadata)
        {
            var criterionVisitor = new NHibernateCriterionVisitor(criteria, filterClause, metadata);
            var criterion = criterionVisitor.Visit(filterClause.Expression);
            if (null != criterion) {
                criteria.Add(criterion);
            }
            return criteria;
        }

        internal static ICriteria OrderContentQuery(this ICriteria criteria, OrderByClause orderByClause, IODataServiceMetadata metadata)
        {
            if (null == orderByClause) {
                return criteria;
            }

            var projectionVisitor = new NHibernateProjectionVisitor(criteria, orderByClause, metadata);
            var projection = projectionVisitor.Visit(orderByClause.Expression);
            
            if (null != projection && orderByClause.Direction == OrderByDirection.Ascending) {
                criteria.AddOrder(Order.Asc(projection));
            }
            
            if (null != projection && orderByClause.Direction == OrderByDirection.Descending) {
                criteria.AddOrder(Order.Desc(projection));
            }

            return criteria.OrderContentQuery(orderByClause.ThenBy, metadata);
        }
    }
}