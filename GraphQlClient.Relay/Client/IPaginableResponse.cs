using System.Threading.Tasks;

namespace GraphQlClient.Relay
{
    public interface IPaginableResponse<T>
    {
        string GetNextPageQueryString(T result);
        bool HasNextPage(T result);
    }
}
