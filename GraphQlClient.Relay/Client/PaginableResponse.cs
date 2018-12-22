using GraphQlClient.Relay.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQlClient.Relay.Client
{
    internal class PaginableResponse<T> : IPaginableResponse<T>
    {
        private IGraphQueryableObject _query;
        private Dictionary<int, IGraphQueryableObject> _currentConnections;

        public PaginableResponse(IGraphQueryableObject query)
        {
            _query = query;
            _currentConnections = new Dictionary<int, IGraphQueryableObject>();
        }

        public string GetNextPageQueryString(T result)
        {
            var (level, path, connection) = FindDeepestConnection(result, 0, string.Empty, (null, null, null));

            if (connection == null)
            {
                return null;
            }

            var keysToRemove = new List<int>();
            foreach (var connectionEntry in _currentConnections.Where(x => x.Key >= level))
            {
                ((GraphQueryableObject<object>)connectionEntry.Value).RemoveArgument("after");
                keysToRemove.Add(connectionEntry.Key);
            }

            foreach (var key in keysToRemove)
            {
                _currentConnections.Remove(key);
            }

            var queryableField = (IGraphQueryableObject)_query.SearchField(path);
            queryableField.AddArgument("after", connection.Edges.Last().Cursor);

            var stringQuery = ((IGraphQueryable)_query).ToQueryString();

            _currentConnections.Add(level.Value, queryableField);

            return stringQuery;
        }

        private (int? Level, string path, IConnection Connection) FindDeepestConnection(object container, int level, string path, (int? Level, string path, IConnection Connection) candidate)
        {
            if (container == null)
            {
                return candidate;
            }

            if (container.GetType().GetInterface(nameof(IEnumerable)) != null)
            {
                var elementType = container.GetType().IsArray
                    ? container.GetType().GetElementType()
                    : container.GetType().GetGenericArguments().SingleOrDefault();

                if (elementType == null || elementType.IsValueType)
                {
                    return candidate;
                }

                foreach (var item in (IEnumerable)container)
                {
                    var newCandidate = FindDeepestConnection(item, level, path, candidate);
                    if (newCandidate.Connection != null)
                    {
                        if (candidate.Connection == null || candidate.Level.Value <= newCandidate.Level.Value)
                        {
                            candidate = newCandidate;
                        }
                    }
                }

                return candidate;
            }

            if (container.GetType().GetInterface(nameof(IConnection)) != null)
            {
                var pageInfo = ((IConnection)container).PageInfo;
                if (pageInfo.HasNextPage)
                {
                    if (candidate.Connection == null || candidate.Level.Value <= level)
                    {
                        candidate = (level, path, (IConnection)container);
                    }
                }

                container = container.GetType().GetProperty(nameof(IConnection.Edges), BindingFlags.Instance | BindingFlags.Public).GetValue(container);
                var newPath = $"{path}{(string.IsNullOrEmpty(path) ? "Edges" : ".Edges")}";
                var newCandidate = FindDeepestConnection(container, level + 1, newPath, candidate);
                if (newCandidate.Connection != null)
                {
                    if (candidate.Connection == null || candidate.Level.Value <= newCandidate.Level)
                    {
                        candidate = newCandidate;
                    }
                }

                return candidate;
            }

            foreach (var propertyInfo in container.GetType().GetProperties().Where(p => !p.PropertyType.IsValueType))
            {
                var newPath = $"{path}{(string.IsNullOrEmpty(path) ? propertyInfo.Name : $".{propertyInfo.Name}")}";
                var newCandidate = FindDeepestConnection(propertyInfo.GetValue(container), level + 1, newPath, candidate);
                if (newCandidate.Connection != null)
                {
                    if (candidate.Connection == null || candidate.Level.Value <= newCandidate.Level.Value)
                    {
                        candidate = newCandidate;
                    }
                }
            }

            return candidate;
        }

        public bool HasNextPage(T result)
        {
            return HasNextPage(result);
        }

        private bool HasNextPage(object container)
        {
            if (container == null)
            {
                return false;
            }

            if (container.GetType().IsArray)
            {
                if (container.GetType().GetElementType().IsValueType)
                {
                    return false;
                }

                foreach (var item in (IEnumerable)container)
                {
                    if (HasNextPage(item))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (container.GetType().GetInterface(nameof(IConnection)) != null)
            {
                var pageInfo = ((IConnection)container).PageInfo;
                if (pageInfo.HasNextPage)
                {
                    return true;
                }

                return HasNextPage(container.GetType().GetProperty(nameof(IConnection.Edges), BindingFlags.Instance | BindingFlags.Public).GetValue(container));
            }

            foreach (var propertyInfo in container.GetType().GetProperties().Where(p => !p.PropertyType.IsValueType))
            {
                if (HasNextPage(propertyInfo.GetValue(container)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
