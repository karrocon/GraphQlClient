using System;
using System.Linq.Expressions;

namespace GraphQlClient
{
    public interface IGraphQueryableObject<TEntity> : IGraphQueryableObject
    {
        IGraphQueryableObject<TEntity> AddArgument<TArgument>(string argumentName, TArgument value);
        IGraphQueryableObject<TEntity> AddArgumentAsVariable(string argumentName, string variableName);
        IGraphQueryableObject<TEntity> AddObject<TProperty>(Expression<Func<TEntity, TProperty>> navigationFcn, Action<IGraphQueryableObject<TProperty>> innerGraphQueryable);
        IGraphQueryableObject<TEntity> AddObjectAs<TProperty, TInnerProperty>(Expression<Func<TEntity, TProperty>> navigationFcn, Action<IGraphQueryableObject<TInnerProperty>> innerGraphQueryable);
        IGraphQueryableObject<TEntity> AddScalar<TProperty>(Expression<Func<TEntity, TProperty>> navigationFcn);
        IGraphQueryableObject<TEntity> IncludeAllScalars();
    }

    public interface IGraphQueryableObject
    {
        IGraphQueryableObject AddArgument(string argumentName, object value);
        IGraphQueryable SearchField(string path);
    }
}
