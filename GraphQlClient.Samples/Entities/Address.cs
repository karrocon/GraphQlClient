namespace GraphQlClient.Samples
{
    public enum AddressType { Avenue, Square, Street, Way }

    public class Address
    {
        public AddressType Type { get; set; }
        public string Name { get; set; }
        public int? Number { get; set; }
        public string OtherInformation { get; set; }
        public Shop Shop { get; set; }
        public User User { get; set; }
    }
}
