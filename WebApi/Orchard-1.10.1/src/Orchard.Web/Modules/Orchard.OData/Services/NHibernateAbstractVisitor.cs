
namespace Orchard.OData.Services
{
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using NHibernate;

    internal abstract class NHibernateAbstractVisitor<T> : QueryNodeVisitor<T>
    {
        protected readonly ICriteria contentItemVersionRecordCriteria;
        protected readonly ICriteria contentItemRecordCriteria;
        protected readonly IODataServiceMetadata metadata;

        protected NHibernateAbstractVisitor(ICriteria contentItemVersionRecordCriteria, IODataServiceMetadata metadata)
            : base()
        {
            this.contentItemVersionRecordCriteria = contentItemVersionRecordCriteria;
            this.contentItemRecordCriteria = contentItemVersionRecordCriteria.BindCriteriaByPath("ContentItemRecord");
            this.metadata = metadata;
        }

        public bool IsLogicalOperator(BinaryOperatorKind nodeKind)
        {
            return
                nodeKind == BinaryOperatorKind.Or ||
                nodeKind == BinaryOperatorKind.And;
        }

        public bool IsComparisonOperator(BinaryOperatorKind nodeKind)
        {
            return
                nodeKind == BinaryOperatorKind.Equal ||
                nodeKind == BinaryOperatorKind.GreaterThan ||
                nodeKind == BinaryOperatorKind.GreaterThanOrEqual ||
                nodeKind == BinaryOperatorKind.LessThan ||
                nodeKind == BinaryOperatorKind.LessThanOrEqual ||
                nodeKind == BinaryOperatorKind.NotEqual;
        }

        public bool IsArithmeticalOperator(BinaryOperatorKind nodeKind)
        {
            return
                nodeKind == BinaryOperatorKind.Add ||
                nodeKind == BinaryOperatorKind.Divide ||
                nodeKind == BinaryOperatorKind.Modulo ||
                nodeKind == BinaryOperatorKind.Multiply ||
                nodeKind == BinaryOperatorKind.Subtract;
        }

        public bool IsNullNode(QueryNode singlevalueNode)
        {
            return
                singlevalueNode.Kind == QueryNodeKind.None ||
                (singlevalueNode is ConstantNode && (singlevalueNode as ConstantNode).Value == null);
        }
    }
}