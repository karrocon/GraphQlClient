using System.Threading.Tasks;

namespace GraphQlClient.Relay.Client
{
    public interface IPaginableResponse<T>
    {
        string GetNextPageQueryString();
        bool HasNextPage();
        Task<T> ReadAsStringAsync();
    }
}
