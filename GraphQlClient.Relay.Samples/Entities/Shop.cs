namespace GraphQlClient.Relay.Samples
{
    public class Shop
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public Connection<User> Users { get; set; }
    }
}
