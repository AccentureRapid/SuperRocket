
namespace Orchard.OData.Services
{
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using NHibernate;
    using NHibernate.Criterion;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class NHibernateCriterionVisitor : NHibernateAbstractVisitor<ICriterion>
    {
        private readonly Dictionary<string, Func<IEnumerable<QueryNode>, ICriterion>> criterionMethodVisitorFactory;
        private readonly NHibernateProjectionVisitor projectionVisitor;
        private readonly FilterClause filterClause;

        internal NHibernateCriterionVisitor(ICriteria contentItemVersionRecordCriteria, FilterClause filterClause, IODataServiceMetadata metadata)
            : base(contentItemVersionRecordCriteria, metadata)
        {
            this.filterClause = filterClause;
            this.projectionVisitor = new NHibernateProjectionVisitor(contentItemVersionRecordCriteria, filterClause, metadata);
            this.criterionMethodVisitorFactory = new Dictionary<string, Func<IEnumerable<QueryNode>, ICriterion>>() {
                {"substring", this.SubStringOfMethod},
                {"startswith", this.StartsWithMethod},
                {"endswith", this.EndsWithMethod},
            };
        }

        public ICriterion Visit(QueryNode queryNode)
        {
            return queryNode.Accept<ICriterion>(this);
        }

        public override ICriterion Visit(SingleValueFunctionCallNode nodeIn)
        {
            var methodName = nodeIn.Name.ToLower();
            if (!this.criterionMethodVisitorFactory.ContainsKey(methodName)) {
                return null;
            }
            var method = this.criterionMethodVisitorFactory[methodName];
            return method(nodeIn.Arguments);
        }

        public override ICriterion Visit(BinaryOperatorNode nodeIn)
        {
            var operatorKind = nodeIn.OperatorKind;
            var leftNode = nodeIn.Left;
            var rightNode = nodeIn.Right;

            if (this.IsNullNode(leftNode) && this.IsNullNode(rightNode)) {
                return null;
            }

            // Invert if left is null
            if (this.IsNullNode(leftNode)) {
                rightNode = nodeIn.Left;
                leftNode = nodeIn.Right;
            }

            if (this.IsNullNode(rightNode)) {
                var leftProjection = this.projectionVisitor.Visit(leftNode);
                if (null == leftProjection) {
                    return null;
                }

                switch (operatorKind) {
                    case BinaryOperatorKind.Equal: return Restrictions.IsNull(leftProjection);
                    case BinaryOperatorKind.NotEqual: return Restrictions.IsNotNull(leftProjection);
                    default: return null;
                }
            }

            if (this.IsComparisonOperator(operatorKind)) {
                var leftProjection = this.projectionVisitor.Visit(leftNode);
                var rightProjection = this.projectionVisitor.Visit(rightNode);
                if (false == (null !=leftProjection && null!=rightProjection)) {
                    return null;
                }

                switch (operatorKind) {
                    case BinaryOperatorKind.Equal : return Restrictions.EqProperty(leftProjection, rightProjection);
                    case BinaryOperatorKind.GreaterThan: return Restrictions.GtProperty(leftProjection, rightProjection);
                    case BinaryOperatorKind.GreaterThanOrEqual: return Restrictions.GeProperty(leftProjection, rightProjection);
                    case BinaryOperatorKind.LessThan : return Restrictions.LtProperty(leftProjection, rightProjection);
                    case BinaryOperatorKind.LessThanOrEqual: return Restrictions.LeProperty(leftProjection, rightProjection);
                    default: return null;
                }
            }

            if (this.IsLogicalOperator(operatorKind)) {
                var leftCriterion = leftNode.Accept<ICriterion>(this);
                var rightCriterion = rightNode.Accept<ICriterion>(this);
                if (false == (null != leftCriterion && null != rightCriterion)) {
                    return null;
                }

                switch (operatorKind) {
                    case BinaryOperatorKind.And: return Restrictions.And(leftCriterion, rightCriterion);
                    case BinaryOperatorKind.Or: return Restrictions.Or(leftCriterion, rightCriterion);
                }
            }

            return null;
        }

        public override ICriterion Visit(UnaryOperatorNode nodeIn)
        {
            if (nodeIn.OperatorKind != UnaryOperatorKind.Not) {
                return null;
            }

            var criterion = nodeIn.Operand.Accept<ICriterion>(this);
            if (null == criterion) {
                return null;
            }

            return Restrictions.Not(criterion);
        }

        public ICriterion SubStringOfMethod(IEnumerable<QueryNode> queryNode)
        {
            return this.StringMethod(queryNode, MatchMode.Anywhere);
        }

        public ICriterion StartsWithMethod(IEnumerable<QueryNode> queryNode)
        {
            return this.StringMethod(queryNode, MatchMode.Start);
        }

        public ICriterion EndsWithMethod(IEnumerable<QueryNode> queryNode)
        {
            return this.StringMethod(queryNode, MatchMode.End);
        }

        private ICriterion StringMethod(IEnumerable<QueryNode> queryNode, MatchMode matchMode)
        {
            if (queryNode.Count() != 2)
            {
                return null;
            }

            var arguments = queryNode.ToList();
            var leftNode = arguments[0];
            var rightNode = arguments[1];

            if (leftNode is ConstantNode)
            {
                rightNode = arguments[0];
                leftNode = arguments[1];
            }

            if (false == rightNode is ConstantNode)
            {
                return null;
            }

            var leftProjection = this.projectionVisitor.Visit(leftNode);
            if (null == leftProjection)
            {
                return null;
            }
            return Restrictions.Like(leftProjection, (rightNode as ConstantNode).Value.ToString(), matchMode);
        }
    }
}