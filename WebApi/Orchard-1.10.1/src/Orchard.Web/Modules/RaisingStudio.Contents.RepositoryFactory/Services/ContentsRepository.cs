using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Orchard.Utility.Extensions;
using System.Web;
using System.Data.Linq.Mapping;
using System.Reflection;
using RaisingStudio.Data;
using Orchard.Core.Common.Fields;
using Orchard.Fields.Fields;
using Orchard.ContentPicker.Fields;
using System.Collections.Concurrent;

namespace RaisingStudio.Contents.RepositoryFactory.Services
{
    public class ContentsRepository<T> : IContentsRepository<T> where T : class, new()
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;

        public ContentsRepository(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            string contentTypeName)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;

            _contentTypeName = contentTypeName;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }


        public static IEnumerable<Action<S, T>> BuildPropertyTransfers<S, T>()
        {
            List<Action<S, T>> propertyTransferList = new List<Action<S, T>>();
            Type sourceType = typeof(S);
            Type targetType = typeof(T);
            var properties = sourceType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                string propertyName = propertyInfo.Name;
                var targetPropertyInfo = targetType.GetProperty(propertyName);
                if (targetPropertyInfo != null)
                {
                    var source = Expression.Parameter(sourceType, "source");
                    var target = Expression.Parameter(targetType, "target");
                    var source_get_call = Expression.Call(source, propertyInfo.GetGetMethod());
                    var target_set_call = Expression.Call(target, targetPropertyInfo.GetSetMethod(), source_get_call);
                    var expression = Expression.Lambda<Action<S, T>>(target_set_call, source, target);
                    var action = expression.Compile();
                    propertyTransferList.Add(action);
                }
            }
            return propertyTransferList;
        }

        private static ConcurrentDictionary<Type, object> propertyTransfers = new ConcurrentDictionary<Type, object>();
        public static IEnumerable<Action<T, T>> GetPropertyTransfers<T>()
        {
            return propertyTransfers.GetOrAdd(typeof(T), _ => { return BuildPropertyTransfers<T, T>(); }) as IEnumerable<Action<T, T>>;
        }

        private IEnumerable<Action<T, T>> _propertyTransfers;
        public IEnumerable<Action<T, T>> GetPropertyTransfers()
        {
            if (_propertyTransfers == null)
            {
                lock (this)
                {
                    if (_propertyTransfers == null)
                    {
                        _propertyTransfers = GetPropertyTransfers<T>();
                    }
                }
            }
            return _propertyTransfers;
        }

        private string _contentTypeName;
        public string GetContentTypeName()
        {
            if(!string.IsNullOrEmpty(_contentTypeName))
            {
                return _contentTypeName;
            }
            bool hasTableAttribute;
            return EntityAdapter.GetTableName(typeof(T), out hasTableAttribute);
        }

        private ColumnAttribute[] _propertyAttributes;
        private string[] _propertyNames;
        private Type[] _propertyTypes;
        private string[] _columnNames;
        private string[] _columnTypes;


        private Func<object, object>[] _propertySetterConverters;
        private Action<T, object>[] _propertySetters;
        public static Action<T, object>[] GetPropertySetters(out ColumnAttribute[] propertyAttributes, out string[] propertyNames, out Type[] propertyTypes, out string[] columnNames, out string[] columnTypes, out Func<object, object>[] converters)
        {
            Type entityType = typeof(T);
            EntityAdapter.GetEntityMapping(entityType, out propertyAttributes, out propertyNames, out propertyTypes, out columnNames, out columnTypes);
            return GetPropertySetters(propertyNames, propertyTypes, columnTypes, out converters);
        }
        public static Action<T, object>[] GetPropertySetters(string[] propertyNames, Type[] propertyTypes, string[] columnTypes, out Func<object, object>[] converters)
        {
            Type entityType = typeof(T);
            int propertyCount = propertyNames.Length;
            converters = new Func<object, object>[propertyCount];
            var propertySetters = new Action<T, object>[propertyCount];
            for (int i = 0; i < propertyCount; i++)
            {
                var instance = Expression.Parameter(typeof(T), "instance");
                var value = Expression.Parameter(typeof(object), "value");
                var convert = Expression.Convert(value, propertyTypes[i]);
                var call = Expression.Call(instance, entityType.GetProperty(propertyNames[i]).GetSetMethod(), convert);
                var expression = Expression.Lambda<Action<T, object>>(call, instance, value);
                var action = expression.Compile();
                propertySetters[i] = action;
                converters[i] = ConverterManager.Default.GetConverter(columnTypes[i], propertyTypes[i]);
            }
            return propertySetters;
        }
        public Action<T, object>[] PropertySetters
        {
            get
            {
                if (_propertySetters == null)
                {
                    if (_propertyAttributes == null)
                    {
                        _propertySetters = GetPropertySetters(out _propertyAttributes, out _propertyNames, out _propertyTypes, out _columnNames, out _columnTypes, out _propertySetterConverters);
                    }
                    else
                    {
                        _propertySetters = GetPropertySetters(_propertyNames, _propertyTypes, _columnTypes, out _propertySetterConverters);

                    }
                }
                return _propertySetters;
            }
        }

        private Func<object, object>[] _propertyGetterConverters;
        private Func<T, object>[] _propertyGetters;
        public static Func<T, object>[] GetPropertyGetters(out ColumnAttribute[] propertyAttributes, out string[] propertyNames, out Type[] propertyTypes, out string[] columnNames, out string[] columnTypes, out Func<object, object>[] converters)
        {
            Type entityType = typeof(T);
            EntityAdapter.GetEntityMapping(entityType, out propertyAttributes, out propertyNames, out propertyTypes, out columnNames, out columnTypes);
            return GetPropertyGetters(propertyNames, propertyTypes, columnTypes, out converters);
        }
        public static Func<T, object>[] GetPropertyGetters(string[] propertyNames, Type[] propertyTypes, string[] columnTypes, out Func<object, object>[] converters)
        {
            Type entityType = typeof(T);
            int propertyCount = propertyNames.Length;
            converters = new Func<object, object>[propertyCount];
            var propertyGetters = new Func<T, object>[propertyCount];
            for (int i = 0; i < propertyCount; i++)
            {
                var instance = Expression.Parameter(typeof(T), "instance");
                var call = Expression.Call(instance, entityType.GetProperty(propertyNames[i]).GetGetMethod());
                var convert = Expression.Convert(call, typeof(object));
                var expression = Expression.Lambda<Func<T, object>>(convert, instance);
                var func = expression.Compile();
                propertyGetters[i] = func;
                converters[i] = ConverterManager.Default.GetConverter(propertyTypes[i], columnTypes[i]);
            }
            return propertyGetters;
        }
        public Func<T, object>[] PropertyGetters
        {
            get
            {
                if (_propertyGetters == null)
                {
                    if (_propertyAttributes == null)
                    {
                        _propertyGetters = GetPropertyGetters(out _propertyAttributes, out _propertyNames, out _propertyTypes, out _columnNames, out _columnTypes, out _propertyGetterConverters);
                    }
                    else
                    {
                        _propertyGetters = GetPropertyGetters(_propertyNames, _propertyTypes, _columnTypes, out _propertyGetterConverters);

                    }
                }
                return _propertyGetters;
            }
        }


        private object GetContentValue(ContentItem contentItem, string fieldName, string dataType, Func<object, object> converter)
        {
            if (fieldName == "Id")
            {
                return contentItem.Id;
            }
            else
            {
                var contentPart = contentItem.Parts.SingleOrDefault(p => p.PartDefinition.Name == contentItem.TypeDefinition.Name);
                if ((contentPart != null) && (contentPart.Fields.Count() > 0))
                {
                    var field = contentPart.Fields.FirstOrDefault(f => f.Name == fieldName);
                    if (field != null)
                    {
                        return GetContentFieldValue(field, dataType, converter);
                    }
                }
            }
            return null;
        }

        private object GetContentFieldValue(ContentField field, string dataType, Func<object, object> converter)
        {
            object value = null;
            #region Check Fields
            if (field is TextField)
            {
                value = (field as TextField).Value;
            }
            else if (field is BooleanField)
            {
                value = (field as BooleanField).Value;
            }
            else if (field is DateTimeField)
            {
                value = (field as DateTimeField).DateTime;
            }
            else if (field is EnumerationField)
            {
                value = (field as EnumerationField).Value;
            }
            else if (field is InputField)
            {
                value = (field as InputField).Value;
            }
            else if (field is LinkField)
            {
                value = (field as LinkField).Value;
            }
            //else if (field is MediaPickerField)
            //{
            //    value = (field as MediaPickerField).Url;
            //}
            else if (field is NumericField)
            {
                value = (field as NumericField).Value;
            }
            else if (field is ContentPickerField)
            {
                //value = (field as ContentPickerField).Ids;
                // for IComparable.
                value = string.Join(",", (field as ContentPickerField).Ids);
            }
            #endregion
            if (value != null)
            {
                if (converter == null)
                {
                    converter = ConverterManager.Default.GetConverter(value.GetType(), dataType);
                }
                if (converter != null)
                {
                    value = converter(value);
                }
            }
            return value;
        }


        private void SetContentValue(ContentItem contentItem, object value, string fieldName, string dataType, Func<object, object> converter)
        {
            if (fieldName == "Id")
            {
                // TODO:
            }
            else
            {
                var contentPart = contentItem.Parts.SingleOrDefault(p => p.PartDefinition.Name == contentItem.TypeDefinition.Name);
                if ((contentPart != null) && (contentPart.Fields.Count() > 0))
                {
                    var field = contentPart.Fields.FirstOrDefault(f => f.Name == fieldName);
                    if (field != null)
                    {
                        SetContentFieldValue(field, value);
                    }
                }
            }
        }

        private void SetContentFieldValue(ContentField field, object value)
        {
            #region Check Fields
            if (field is TextField)
            {
                (field as TextField).Value = Convert.ToString(value);
            }
            else if (field is BooleanField)
            {
                if (value != null)
                {
                    if (value is string)
                    {
                        (field as BooleanField).Value = string.IsNullOrWhiteSpace((string)value) ? null : (bool?)Convert.ToBoolean(value);
                    }
                    else if (value is bool)
                    {
                        (field as BooleanField).Value = (bool)value;
                    }
                    else if (value is bool?)
                    {
                        (field as BooleanField).Value = (bool?)value;
                    }
                    else
                    {
                        (field as BooleanField).Value = Convert.ToBoolean(value);
                    }
                }
                else
                {
                    (field as BooleanField).Value = null;
                }
            }
            else if (field is DateTimeField)
            {
                (field as DateTimeField).DateTime = Convert.ToDateTime(value);
            }
            else if (field is EnumerationField)
            {
                (field as EnumerationField).Value = Convert.ToString(value);
            }
            else if (field is InputField)
            {
                (field as InputField).Value = Convert.ToString(value);
            }
            else if (field is LinkField)
            {
                (field as LinkField).Value = Convert.ToString(value);
            }
            //else if (field is MediaPickerField)
            //{
            //    (field as MediaPickerField).Url = Convert.ToString(value);
            //}
            else if (field is NumericField)
            {
                if (value != null)
                {
                    if (value is string)
                    {
                        (field as NumericField).Value = string.IsNullOrWhiteSpace((string)value) ? null : (decimal?)Convert.ToDecimal(value);
                    }
                    else if (value is decimal)
                    {
                        (field as NumericField).Value = (decimal)value;
                    }
                    else if (value is decimal?)
                    {
                        (field as NumericField).Value = (decimal?)value;
                    }
                    else
                    {
                        (field as NumericField).Value = Convert.ToDecimal(value);
                    }
                }
                else
                {
                    (field as NumericField).Value = null;
                }
            }
            else if (field is ContentPickerField)
            {
                if (value != null)
                {
                    if (value is int[])
                    {
                        (field as ContentPickerField).Ids = (int[])value;
                    }
                    else if (value is string)
                    {
                        if (string.IsNullOrWhiteSpace((string)value))
                        {
                            (field as ContentPickerField).Ids = new int[0];
                        }
                        else
                        {
                            char[] separator = new[] { '{', '}', ',' };
                            (field as ContentPickerField).Ids = ((string)value).Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                        }
                    }
                    else if (value is string[])
                    {
                        (field as ContentPickerField).Ids = ((string[])value).Select(int.Parse).ToArray();
                    }
                    else if (value is object[])
                    {
                        (field as ContentPickerField).Ids = ((object[])value).Select(Convert.ToInt32).ToArray();
                    }
                    //else if (value is JArray)
                    //{
                    //    (field as ContentPickerField).Ids = ((JArray)value).Select((jValue) => (int)jValue).ToArray();
                    //}
                    else
                    {
                        throw new NotSupportedException(string.Format("{0} is not supported for ContentPickerField", value));
                    }
                }
                else
                {
                    (field as ContentPickerField).Ids = null;
                }
            }
            #endregion
        }


        private T ConvertToEntity(ContentItem contentItem, Action<T, object>[] propertySetters, string[] columnNames, string[] columnTypes, Func<object, object>[] converters)
        {
            T entity = new T();
            for (int i = 0; i < propertySetters.Length; i++)
            {
                var value = GetContentValue(contentItem, columnNames[i], columnTypes[i], converters[i]);
                propertySetters[i](entity, value);
            }
            return entity;
        }

        private ContentItem ConvertToContentItem(T entity, ContentItem contentItem, Func<T, object>[] propertyGetters, string[] columnNames, string[] columnTypes, Func<object, object>[] converters)
        {
            for (int i = 0; i < propertyGetters.Length; i++)
            {
                var value = propertyGetters[i](entity);
                SetContentValue(contentItem, value, columnNames[i], columnTypes[i], converters[i]);
            }
            return contentItem;
        }

        private void ConvertToContentItem(T entity, ContentItem contentItem)
        {
            var propertyGetters = PropertyGetters;
            ConvertToContentItem(entity, contentItem, propertyGetters, _columnNames, _columnTypes, _propertyGetterConverters);
        }


        public virtual int GetId(T entity)
        {
            dynamic t = entity;
            return (int)t.Id;
        }

        public virtual T SetId(T entity, int id)
        {
            dynamic t = entity;
            t.Id = id;
            return (T)t;
        }


        public virtual IQueryable<T> Table
        {
            get
            {
                var propertySetters = PropertySetters;
                var query = from contentItem in _contentManager.Query(VersionOptions.Published, GetContentTypeName()).List().AsQueryable()
                            select ConvertToEntity(contentItem, propertySetters, _columnNames, _columnTypes, _propertySetterConverters);
                return query;
            }
        }


        #region IRepository<T> Members

        void IRepository<T>.Create(T entity)
        {
            Create(entity);
        }

        void IRepository<T>.Update(T entity)
        {
            Update(entity);
        }

        void IRepository<T>.Delete(T entity)
        {
            Delete(entity);
        }

        void IRepository<T>.Copy(T source, T target)
        {
            Copy(source, target);
        }

        void IRepository<T>.Flush()
        {
            Flush();
        }

        T IRepository<T>.Get(int id)
        {
            return Get(id);
        }

        T IRepository<T>.Get(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate);
        }

        IQueryable<T> IRepository<T>.Table
        {
            get { return Table; }
        }

        int IRepository<T>.Count(Expression<Func<T, bool>> predicate)
        {
            return Count(predicate);
        }

        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).ToReadOnlyCollection();
        }

        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            return Fetch(predicate, order).ToReadOnlyCollection();
        }

        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                            int count)
        {
            return Fetch(predicate, order, skip, count).ToReadOnlyCollection();
        }

        #endregion


        public virtual T Get(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.AllVersions);
            if (contentItem != null)
            {
                var propertySetters = PropertySetters;
                return ConvertToEntity(contentItem, propertySetters, _columnNames, _columnTypes, _propertySetterConverters);
            }
            return default(T);
        }

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).SingleOrDefault();
        }

        public virtual void Create(T entity)
        {
            Logger.Debug("Create {0}", entity);
            var contentItem = _contentManager.New(GetContentTypeName());
            _contentManager.Create(contentItem, VersionOptions.Draft);
            SetId(entity, contentItem.Id);
            ConvertToContentItem(entity, contentItem);
            _contentManager.Publish(contentItem);
        }

        public virtual void Update(T entity)
        {
            Logger.Debug("Update {0}", entity);
            var contentItem = _contentManager.Get(GetId(entity), VersionOptions.DraftRequired);
            if (contentItem != null)
            {
                ConvertToContentItem(entity, contentItem);
                _contentManager.Publish(contentItem);
            }
        }

        public virtual void Delete(T entity)
        {
            Logger.Debug("Delete {0}", entity);
            _contentManager.Remove(_contentManager.Get(GetId(entity)));
        }

        public virtual void Copy(T source, T target)
        {
            Logger.Debug("Copy {0} {1}", source, target);
            IEnumerable<Action<T, T>> propertyTransferList = GetPropertyTransfers();
            foreach (var propertyTransfer in propertyTransferList)
            {
                propertyTransfer(source, target);
            }
        }

        public virtual void Flush()
        {
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
        {
            return Table.Where(predicate);
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            var orderable = new Orderable<T>(Fetch(predicate));
            order(orderable);
            return orderable.Queryable;
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                           int count)
        {
            return Fetch(predicate, order).Skip(skip).Take(count);
        }
    }
}