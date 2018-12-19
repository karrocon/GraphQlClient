using GraphQlClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GraphQlClient.Relay")]
namespace GraphQlClient
{
    internal class GraphQueryableObject<TEntity> : GraphQueryableField, IGraphQueryableObject<TEntity>
    {
        #region Properties

        Dictionary<string, GraphQueryableArgument> _arguments;
        Dictionary<string, GraphQueryableField> _fields;

        #endregion

        #region Constructors

        public GraphQueryableObject(string name) : base(name)
        {
            _arguments = new Dictionary<string, GraphQueryableArgument>();
            _fields = new Dictionary<string, GraphQueryableField>();
        }

        #endregion

        #region IGraphQueryableObject implementation

        IGraphQueryableObject IGraphQueryableObject.AddArgument(string argumentName, object value)
        {
            AddArgument(argumentName, value);

            return this;
        }

        public IGraphQueryable SearchField(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var splittedPath = path.Split('.');
            var nextPath = splittedPath[0];
            if (!_fields.ContainsKey(nextPath) && _fields.ContainsKey(nextPath.ToLowerInvariantFirst()))
            {
                // TODO: This should not be updated here by default. The parser should receive the expected
                // type to properly perform the Parse function from the model
                nextPath = nextPath.ToLowerInvariantFirst();
            }
            else if (!_fields.ContainsKey(nextPath))
            {
                throw new KeyNotFoundException("There are no items matching the given path");
            }

            if (splittedPath.Length == 1)
            {
                return _fields[nextPath];
            }

            var field = _fields[nextPath];
            if (field == null || field.GetType().GetInterface(nameof(IGraphQueryableObject)) == null)
            {
                throw new KeyNotFoundException("There are no items matching the given path");
            }

            return ((IGraphQueryableObject)field).SearchField(path.Remove(0, nextPath.Length + 1));
        }

        #endregion

        #region IGraphQueryableObject<TEntity> implementation

        public IGraphQueryableObject<TEntity> AddArgument<TArgument>(string argumentName, TArgument value)
        {
            _arguments[argumentName] = new GraphQueryableArgument(argumentName, new GraphQueryableLiteral<TArgument>(value));

            return this;
        }

        public IGraphQueryableObject<TEntity> AddArgumentAsVariable(string argumentName, string variableName)
        {
            _arguments[argumentName] = new GraphQueryableArgument(argumentName, new GraphQueryableVariable(variableName));

            return this;
        }

        public IGraphQueryableObject<TEntity> AddObject<TProperty>(Expression<Func<TEntity, TProperty>> navigationFcn, Action<IGraphQueryableObject<TProperty>> innerGraphQueryable)
        {
            return AddObjectAs(navigationFcn, innerGraphQueryable);
        }

        public IGraphQueryableObject<TEntity> AddObjectAs<TProperty, TInnerProperty>(Expression<Func<TEntity, TProperty>> navigationFcn, Action<IGraphQueryableObject<TInnerProperty>> innerGraphQueryable)
        {
            var propertyName = GetPropertyNameFromExpression(navigationFcn);
            var newObject = new GraphQueryableObject<TInnerProperty>(propertyName);

            _fields[propertyName] = newObject;

            innerGraphQueryable(newObject);

            return this;
        }

        public IGraphQueryableObject<TEntity> AddScalar<TProperty>(Expression<Func<TEntity, TProperty>> navigationFcn)
        {
            if (navigationFcn == null)
            {
                throw new ArgumentNullException(nameof(navigationFcn));
            }

            var propertyName = GetPropertyNameFromExpression(navigationFcn);
            _fields[propertyName] = new GraphQueryableScalar(propertyName);

            return this;
        }

        public IGraphQueryableObject<TEntity> IncludeAllScalars()
        {
            IncludeAllScalars(typeof(TEntity));

            return this;
        }

        #endregion

        #region GraphQueryableElement implementation

        public override string ToQueryString()
        {
            var argumentsQueryString = _arguments.Any() ? $"({string.Join(",", _arguments.Values.Select(x => x.ToQueryString()))})" : string.Empty;
            var fieldsQueryString = string.Join(", ", _fields.Values.Select(x => x.ToQueryString()));

            return $"{Name.ToLowerInvariantFirst()}{argumentsQueryString} {{ {fieldsQueryString} }}";
        }

        #endregion

        #region Internal methods

        internal IGraphQueryableObject<TEntity> AddArgument(GraphQueryableArgument argument)
        {
            _arguments.Add(argument.Name, argument);

            return this;
        }

        internal IGraphQueryableObject<TEntity> AddScalar(GraphQueryableScalar scalar)
        {
            _fields.Add(scalar.Name, scalar);

            return this;
        }

        internal IGraphQueryableObject<TEntity> AddObject<TObject>(GraphQueryableObject<TObject> @object)
        {
            _fields.Add(@object.Name, @object);

            return this;
        }

        internal IGraphQueryableObject<TEntity> RemoveArgument(string argumentName)
        {
            _arguments.Remove(argumentName);

            return this;
        }

        #endregion

        #region Private helpers

        private string GetPropertyNameFromExpression<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        private void IncludeAllScalars(Type type)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsValueType || property.PropertyType.Equals(typeof(string)))
                {
                    _fields[property.Name] = new GraphQueryableScalar(property.Name);
                }
                else if (property.PropertyType.IsArray)
                {
                    IncludeAllScalars(property.PropertyType.GetElementType());
                }
            }
        }

        #endregion
    }
}
