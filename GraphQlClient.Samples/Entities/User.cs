using System;
using System.Collections.Generic;

namespace GraphQlClient.Samples
{
    public class User
    {
        public IEnumerable<Address> Addresses { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IEnumerable<Shop> Shops { get; set; }
        public string UserName { get; set; }
    }
}
