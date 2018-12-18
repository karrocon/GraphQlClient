using GraphQlClient.Relay.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GraphQlClient.Relay.Client
{
    internal class PaginableResponse<T> : IPaginableResponse<T>
    {
        private IGraphQueryableObject _query;
        private T _result;
        private IGraphQueryableObject _lastConnection;

        public PaginableResponse(IGraphQueryableObject query, T result)
        {
            _query = query;
            _result = result;
            _lastConnection = null;
        }

        public string GetNextPageQueryString()
        {
            var (level, path, connection) = FindDeepestConnection(_result, 0, string.Empty, (null, null, null));

            if (connection == null)
            {
                return null;
            }

            var queryableField = (IGraphQueryableObject)_query.SearchField(path);
            if (_lastConnection != queryableField && _lastConnection != null)
            {
                if (_lastConnection != null)
                {
                    ((GraphQueryableObject<IConnection>)_lastConnection).RemoveArgument("after");
                }
            }
            queryableField.AddArgument("after", connection.Edges.Last().Cursor);

            var stringQuery = ((IGraphQueryable)_query).ToQueryString();

            _lastConnection = queryableField;

            return stringQuery;
        }

        private (int? Level, string path, IConnection Connection) FindDeepestConnection(object container, int level, string path, (int? Level, string path, IConnection Connection) candidate)
        {
            if (container == null)
            {
                return candidate;
            }

            if (container.GetType().IsArray)
            {
                if (container.GetType().GetElementType().IsValueType)
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

        public bool HasNextPage()
        {
            return HasNextPage(_result);
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

        public Task<T> ReadAsStringAsync()
        {
            throw new NotImplementedException();
        }
    }
}
