using Xunit;

namespace GraphQlClient.Test
{
    public class GraphQueryStringParserTest
    {
        [Fact]
        public void ParseShouldCreateStructureIncludingArgumentsWithLiteralValues()
        {
            var argumentName = "FakeArgumentName";
            var argumentValue = "FakeArgumentValue";
            var query = $"{{ Addresses({argumentName}:\"{argumentValue}\") {{ Name }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable) result).ToQueryString().Trim().Replace(" ", string.Empty));
        }

        [Fact]
        public void ParseShouldCreateStructureIncludingArgumentsWithVariableValue()
        {
            var argumentName = "FakeArgumentName";
            var variableName = "FakeVariableName";
            var query = $"{{ Addresses({argumentName}:${variableName}) {{ Name }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable)result).ToQueryString().Trim().Replace(" ", string.Empty));
        }

        [Fact]
        public void ParseShouldCreateStructureIncludingAllScalarFields()
        {
            var query = $"{{ Addresses {{ Type, Name, Number, OtherInformation }} }}";

            var result = GraphQueryStringParser.Parse(query);

            Assert.Equal(query.Replace(" ", string.Empty), ((IGraphQueryable)result).ToQueryString().Trim().Replace(" ", string.Empty));
        }
    }
}
