using GraphQlClient.Relay.Entities;

namespace GraphQlClient.Relay.Samples
{
    public class QueryRoot
    {
        public Connection<Address> Addresses { get; set; }
        public Connection<Shop> Shops { get; set; }
        public Connection<User> Users { get; set; }
    }
}
