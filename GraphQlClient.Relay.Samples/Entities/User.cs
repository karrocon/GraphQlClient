using System;

namespace GraphQlClient.Relay.Samples
{
    public class User
    {
        public Connection<Address> Addresses { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Connection<Shop> Shops { get; set; }
        public string UserName { get; set; }
    }
}
