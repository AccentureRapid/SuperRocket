
namespace Orchard.OData.Services
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Practices.DataServiceProvider;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.OData.Extensions;
    using NHibernate.Type;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.Records;
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Linq;
    using System.Reflection;

    internal sealed class NHibernateProjectionVisitor : NHibernateAbstractVisitor<IProjection>
    {
        private static readonly IType DefaultArithmeticReturnType = NHibernateUtil.Decimal; // Arithmetic operations on two columns default to decimal
        private readonly Dictionary<string, Func<IEnumerable<QueryNode>, IProjection>> projectionMethodVisitorFactory;
        private readonly FilterClause filterClause;
        private readonly OrderByClause orderByClause;

        private NHibernateProjectionVisitor(ICriteria contentItemVersionRecordCriteria, IODataServiceMetadata metadata)
            : base(contentItemVersionRecordCriteria, metadata)
        {
            this.projectionMethodVisitorFactory = new Dictionary<string, Func<IEnumerable<QueryNode>, IProjection>>() 
            {
                {"substring", this.SubStringOfMethod},
                {"concat", this.ConcatMethod},
                {"length", this.LengthMethod},
                {"replace", this.ReplaceMethod},
                {"toupper", this.ToUpperMethod},
                {"tolower", this.ToLowerMethod},
                {"trim", this.TrimMethod},
                {"indexof", this.IndexOfMethod},
                {"ceiling", this.CeilingMethod},
                {"floor", this.FloorMethod},
                {"round", this.RoundMethod},
                {"cast", this.CastMethod},
                {"year", this.YearMethod},
                {"month", this.MonthMethod},
                {"day", this.DayMethod},
                {"minute", this.MinuteMethod},
                {"second", this.SecondMethod},
            };
        }


        internal NHibernateProjectionVisitor(ICriteria contentItemVersionRecordCriteria, OrderByClause orderByClause, IODataServiceMetadata metadata)
            : this(contentItemVersionRecordCriteria, metadata) 
        {
            this.orderByClause = orderByClause;
        }

        internal NHibernateProjectionVisitor(ICriteria contentItemVersionRecordCriteria, FilterClause filterClause, IODataServiceMetadata metadata)
            : this(contentItemVersionRecordCriteria, metadata)
        {
            this.filterClause = filterClause;
        }

        public IProjection Visit(QueryNode queryNode) 
        {
            return queryNode.Accept<IProjection>(this);
        }

        public override IProjection Visit(ConstantNode nodeIn)
        {
            return Projections.Constant(nodeIn.Value, this.nHibernateTypeFromLiteralType(nodeIn.TypeReference));
        }

        public override IProjection Visit(ConvertNode nodeIn)
        {
            var sourceNode = nodeIn.Source;
            if (!(sourceNode is ConstantNode)) {
                return null;
            }

            var constantNode = (ConstantNode)sourceNode;
            var convertedValue = this.ConvertValueFromLiteralValue(nodeIn.TypeReference, constantNode.Value);
            var convertNode = new ConstantNode(convertedValue);
            return convertNode.Accept<IProjection>(this);
        }

        public override IProjection Visit(SingleValuePropertyAccessNode nodeIn)
        {
            var propertyName = nodeIn.Property.Name;
            var declaringSourceNode = nodeIn.Source;
            var declaringTypeReference = declaringSourceNode.TypeReference;
            var declaringTypeName = declaringTypeReference.FullName().Split('.').LastOrDefault();

            ResourceType resourceType;
            if (false == this.metadata.TryResolveResourceType(declaringTypeName, out resourceType)) {
                return null;
            }

            ResourceProperty resourceProperty = resourceType.Properties.FirstOrDefault(prop => prop.Name == propertyName);
            if (null == resourceProperty) {
                return null;
            }

            PropertyInfo instancePropertyInfo = resourceProperty.GetAnnotation().InstanceProperty;
            if (null == instancePropertyInfo) {
                return null;
            }

            if (resourceType.InstanceType == typeof(ContentPart)) {
                var partType = instancePropertyInfo.ReflectedType;
                var recordPropertyInfo = partType.GetProperties().FirstOrDefault(prop => prop.Name == "Record");
                if (null == recordPropertyInfo) {
                    return null;
                }

                var recordType = recordPropertyInfo.PropertyType;
                if (recordType.IsSubclassOf(typeof(ContentPartVersionRecord))){
                    this.contentItemVersionRecordCriteria.BindCriteriaByAlias(recordType.Name, partType.Name);
                }
                else {
                    this.contentItemRecordCriteria.BindCriteriaByAlias(recordType.Name, partType.Name);
                }

                return Projections.Property(partType.Name + "." + propertyName);
            }

            if (!resourceType.InstanceType.IsSubclassOf(typeof(ContentField))) {
                return null;
            }

            var fieldTable = this.FieldTypeFromCLRType(instancePropertyInfo.PropertyType);
            if (string.IsNullOrEmpty(fieldTable)) {
                return null;
            }

            var parentResourceNode = (declaringSourceNode as SingleValuePropertyAccessNode).Source;
            var parentResourceName = parentResourceNode.TypeReference.FullName().Split('.').LastOrDefault();
            this.metadata.TryResolveResourceType(parentResourceName, out resourceType);

            //var resourceSetTypeReference = null != this.filterClause ? this.filterClause.ItemType : this.orderByClause.ItemType;
            var resourceSetTypeName = resourceType.Name; //resourceSetTypeReference.FullName().Split('.').LastOrDefault();
            var aliasJoinName = resourceSetTypeName + fieldTable;

            var criteria = this.contentItemRecordCriteria
                .BindCriteriaByPath("FieldIndexPartRecord")
                .BindCriteriaByAlias(
                    fieldTable,
                    aliasJoinName, 
                    NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                
            if (null != this.filterClause) { 
                criteria.Add(Restrictions.Eq(
                    "PropertyName", 
                    resourceSetTypeName + "." + declaringTypeName + "."));
            }

            return Projections.Property(aliasJoinName + ".Value");
        }

        public override IProjection Visit(SingleValueFunctionCallNode nodeIn)
        {
            var methodName = nodeIn.Name.ToLower();
            if (!this.projectionMethodVisitorFactory.ContainsKey(methodName)) {
                return null;
            }
            var method = this.projectionMethodVisitorFactory[methodName];
            return method(nodeIn.Arguments);
        }

        public override IProjection Visit(BinaryOperatorNode nodeIn)
        {
            var operatorKind = nodeIn.OperatorKind;
            var leftNode = nodeIn.Left;
            var rightNode = nodeIn.Right;

            if (this.IsArithmeticalOperator(operatorKind))
            {
                var leftProjection = leftNode.Accept<IProjection>(this);
                var rightProjection = rightNode.Accept<IProjection>(this);
                var returnType = this.ArtithmicReturnType(leftNode, rightNode);

                if (false == (null != leftProjection && null != rightProjection)) {
                    return null;
                }

                switch (operatorKind)
                {
                    case BinaryOperatorKind.Add: return new ArithmeticOperatorProjection("+", returnType, leftProjection, rightProjection);
                    case BinaryOperatorKind.Subtract: return new ArithmeticOperatorProjection("-", returnType, leftProjection, rightProjection);
                    case BinaryOperatorKind.Multiply: return new ArithmeticOperatorProjection("*", returnType, leftProjection, rightProjection);
                    case BinaryOperatorKind.Divide: return new ArithmeticOperatorProjection("/", returnType, leftProjection, rightProjection);
                    case BinaryOperatorKind.Modulo: return new SqlFunctionProjection("mod", returnType, leftProjection, rightProjection);
                    default: return null;
                }
            }

            return null;
        }

        public IProjection SubStringOfMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();

            if (arguments.Count == 2) {
                return new SqlFunctionProjection(
                    "substring",
                    NHibernateUtil.String,
                    arguments[0].Accept<IProjection>(this),
                    arguments[1].Accept<IProjection>(this)
                );
            }

            return new SqlFunctionProjection(
                "substring",
                NHibernateUtil.String,
                arguments[0].Accept<IProjection>(this),
                arguments[1].Accept<IProjection>(this),
                arguments[2].Accept<IProjection>(this)
            );
        }

         public IProjection ConcatMethod(IEnumerable<QueryNode> queryNode)
         {
            var arguments = queryNode.ToList();

            if (arguments.Count == 1) {
                return arguments[0].Accept<IProjection>(this);
            }

            return new SqlFunctionProjection(
                "concat",
                NHibernateUtil.String,
                arguments[0].Accept<IProjection>(this),
                arguments[1].Accept<IProjection>(this)
            );
        }

        public IProjection LengthMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            if (1 > arguments.Count) {
                return null;
            }

            return new SqlFunctionProjection(
                "length",
                NHibernateUtil.Int32,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection ReplaceMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "replace",
                NHibernateUtil.String,
                arguments[0].Accept<IProjection>(this),
                arguments[1].Accept<IProjection>(this),
                arguments[2].Accept<IProjection>(this)
            );
        }

        public IProjection ToUpperMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "upper",
                NHibernateUtil.String,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection ToLowerMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "lower",
                NHibernateUtil.String,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection TrimMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "trim",
                NHibernateUtil.String,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection IndexOfMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "locate",
                NHibernateUtil.Int32,
                arguments[1].Accept<IProjection>(this),
                arguments[0].Accept<IProjection>(this),
                Projections.Constant(1)
            );
        }

        public IProjection CeilingMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "ceil",
                NHibernateUtil.Int32,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection FloorMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "floor",
                NHibernateUtil.Int32,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection RoundMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            return new SqlFunctionProjection(
                "round",
                NHibernateUtil.Int32,
                arguments[0].Accept<IProjection>(this)
            );
        }

        public IProjection CastMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            
            var projection = arguments[0].Accept<IProjection>(this);
            if (false == arguments[1] is ConstantNode) {
                return null;
            }

            var typeReference = ((ConstantNode)arguments[1]).TypeReference;
            if (typeReference.IsByte() || 
                typeReference.IsSByte() || 
                typeReference.IsInt16() || 
                typeReference.IsInt32() || 
                typeReference.IsInt64()) {
                return new SqlFunctionProjection("round", NHibernateUtil.Int64, projection);
            }
            return projection;
        }

        public IProjection YearMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            return DatePartMethod(arguments, "year");
        }

        public IProjection MonthMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            return DatePartMethod(arguments, "month");
        }

        public IProjection DayMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            return DatePartMethod(arguments, "day");
        }

        public IProjection HourMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            return DatePartMethod(arguments, "hour");
        }

        public IProjection MinuteMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            return DatePartMethod(arguments, "minute");
        }

        public IProjection SecondMethod(IEnumerable<QueryNode> queryNode)
        {
            var arguments = queryNode.ToList();
            return DatePartMethod(arguments, "second");
        }

        private IProjection DatePartMethod(IEnumerable<QueryNode> queryNode, string function)
        {
            var arguments = queryNode.ToList();
            var projection = arguments[0].Accept<IProjection>(this);

            if (false == arguments[1] is ConstantNode) {
                return null;
            }

            return new SqlFunctionProjection(function, NHibernateUtil.Int32, projection);
        }     

        private IType ArtithmicReturnType(QueryNode left, QueryNode right)
        {
            if (left.Kind == QueryNodeKind.Constant)
                return nHibernateTypeFromLiteralType(((ConstantNode)left).TypeReference);
            else if (right.Kind == QueryNodeKind.Constant)
                return nHibernateTypeFromLiteralType(((ConstantNode)right).TypeReference);
            else
                return DefaultArithmeticReturnType;
        }

        private IType nHibernateTypeFromLiteralType(IEdmTypeReference type)
        {
            if (type.IsString()) {
                return NHibernateUtil.String;
            }
            if (type.IsBoolean()) {
                return NHibernateUtil.Boolean;
            }
            if (type.IsSingle()) {
                return NHibernateUtil.Single;
            }
            if (type.IsDouble()) {
                return NHibernateUtil.Double;
            }
            if (type.IsDecimal()) {
                return NHibernateUtil.Decimal;
            }
            if (type.IsInt16()) {
                return NHibernateUtil.Int16;
            }
            if (type.IsInt32()) {
                return NHibernateUtil.Int32;
            }
            if (type.IsInt64()) {
                return NHibernateUtil.Int64;
            }
            if (type.IsBinary()) {
                return NHibernateUtil.Binary;
            }
            if (type.IsDateTime()) {
                return NHibernateUtil.DateTime;
            }
            if (type.IsGuid()) {
                return NHibernateUtil.Guid;
            }
            if (type.IsDateTimeOffset()) {
                return NHibernateUtil.TimeSpan;
            }
            return DefaultArithmeticReturnType;
        }

        private string FieldTypeFromCLRType(Type literalType)
        {
            if (typeof(int) == literalType || typeof(int?) == literalType){
                return "IntegerFieldIndexRecords";
            }
            if (typeof(decimal) == literalType || typeof(decimal?) == literalType) {
                return "DecimalFieldIndexRecords";
            }
            if (typeof(double) == literalType || typeof(double?) == literalType) {
                return "DoubleFieldIndexRecords";
            }
            if (typeof(string) == literalType) {
                return "StringFieldIndexRecords";
            }
            return string.Empty;
        }

        private object ConvertValueFromLiteralValue(IEdmTypeReference convertType, object value)
        {
            if (convertType.IsDouble()) {
                return Convert.ToDouble(value);
            }
            if (convertType.IsInt16()) {
                return Convert.ToInt16(value);
            }
            if (convertType.IsInt16()) {
                return Convert.ToInt32(value);
            }
            if (convertType.IsInt64()) {
                return Convert.ToInt64(value);
            }
            if (convertType.IsDecimal()) {
                return Convert.ToDecimal(value);
            }
            return value.ToString();
        }
    }
}