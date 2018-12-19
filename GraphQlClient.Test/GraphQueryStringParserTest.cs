using Xunit;

namespace GraphQlClient.Test
{
    public class GraphQueryStringParserTest
    {
        [Fact]
        public void ParseShouldCreateStructureIncludingArgumentsWithLiteralValues()
        {
            var argumentName = "fakeArgumentName";
            var argumentValue = "FakeArgumentValue";
            var query = $"{{ addresses({argumentName}:\"{argumentValue}\") {{ name }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable) result).ToQueryString().Trim().Replace(" ", string.Empty));
        }

        [Fact]
        public void ParseShouldCreateStructureIncludingArgumentsWithVariableValue()
        {
            var argumentName = "fakeArgumentName";
            var variableName = "FakeVariableName";
            var query = $"{{ addresses({argumentName}:${variableName}) {{ name }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable)result).ToQueryString().Trim().Replace(" ", string.Empty));
        }

        [Fact]
        public void ParseShouldCreateStructureIncludingAllScalarFields()
        {
            var query = $"{{ addresses {{ type, name, number, otherInformation }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable)result).ToQueryString().Trim().Replace(" ", string.Empty));
        }
    }
}
