using Xunit;

namespace GraphQlClient.Relay.Test
{
    public class GraphQueryStringParserTest
    {
        [Fact]
        public void ParseShouldCreateStructureIncludingAConnection()
        {
            var query = $"{{ addresses(first:5) {{ edges {{ cursor, node {{ type, name, number, otherInformation }} }}, pageInfo {{ hasNextPage, hasPreviousPage }} }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable)result).ToQueryString().Trim().Replace(" ", string.Empty));
        }
    }
}
