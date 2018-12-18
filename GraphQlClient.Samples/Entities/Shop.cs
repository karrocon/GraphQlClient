using System.Collections.Generic;

namespace GraphQlClient.Samples
{
    public class Shop
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
