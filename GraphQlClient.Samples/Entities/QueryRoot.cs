using System.Collections.Generic;

namespace GraphQlClient.Samples
{
    public class QueryRoot
    {
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Shop> Shops { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
