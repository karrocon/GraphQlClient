using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphQlClient.Relay.Extensions
{
    public static class IGraphQueryableExtensions
    {
        public static IGraphQueryableObject<TEntity> AddConnection<TEntity, TNode>(this IGraphQueryableObject<TEntity> graphQueryable, Expression<Func<TEntity, IConnection<TNode>>> navigationFcn, int first, Action<IGraphQueryableObject<TNode>> innerGraphQueryable) where TNode : class
        {
            return AddConnection(graphQueryable, navigationFcn, first, null, innerGraphQueryable);
        }

        public static IGraphQueryableObject<TEntity> AddConnection<TEntity, TNode>(this IGraphQueryableObject<TEntity> graphQueryable, Expression<Func<TEntity, IConnection<TNode>>> navigationFcn, int first, Action<IGraphQueryableObject<IConnection<TNode>>> connectionExpression, Action<IGraphQueryableObject<TNode>> innerGraphQueryable) where TNode : class
        {
            return graphQueryable
                .AddObject(navigationFcn, connectionBuilder =>
                {
                    connectionBuilder
                        .AddObjectAs<IEnumerable<IEdge<TNode>>, IEdge<TNode>>(connection => connection.Edges, edgesBuilder => edgesBuilder
                            .AddScalar(edge => edge.Cursor)
                            .AddObject(edge => edge.Node, innerGraphQueryable)
                        )
                        .AddObject(connection => connection.PageInfo, pageInfoBuilder => pageInfoBuilder
                            .AddScalar(pageInfo => pageInfo.HasNextPage)
                            .AddScalar(pageInfo => pageInfo.HasPreviousPage)
                        )
                        .AddArgument("first", first);
                    connectionExpression?.Invoke(connectionBuilder);
                });
        }
    }
}
