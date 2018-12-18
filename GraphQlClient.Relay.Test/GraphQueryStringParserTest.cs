using Xunit;

namespace GraphQlClient.Relay.Test
{
    public class GraphQueryStringParserTest
    {
        [Fact]
        public void ParseShouldCreateStructureIncludingAConnection()
        {
            var query = $"{{ Addresses(first:5) {{ Edges {{ Cursor, Node {{ Type, Name, Number, OtherInformation }} }}, PageInfo {{ HasNextPage, HasPreviousPage }} }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable)result).ToQueryString().Trim().Replace(" ", string.Empty));
        }
    }
}
