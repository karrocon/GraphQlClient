using System.Threading.Tasks;

namespace GraphQlClient.Relay.Client
{
    public interface IPaginableResponse<T>
    {
        string GetNextPageQueryString(T result);
        bool HasNextPage(T result);
    }
}
