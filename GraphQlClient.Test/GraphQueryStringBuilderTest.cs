using GraphQlClient.Samples;
using System.Collections.Generic;
using Xunit;

namespace GraphQlClient.Test
{
    public class GraphQueryStringBuilderTest
    {
        [Fact]
        public void BuildShouldCreateQueryStringIncludingArgumentsWithLiteralValues()
        {
            var argumentName = "FakeArgumentName";
            var argumentValue = "FakeArgumentValue";
            var expectedArgumentName = $"{char.ToLowerInvariant(argumentName[0])}{argumentName.Substring(1, argumentName.Length - 1)}";
            var expectedQueryString = $"{{ addresses({expectedArgumentName}:\"{argumentValue}\") {{ name }} }}";
            var queryString = GraphQueryStringBuilder.Build<QueryRoot>(rootBuilder => rootBuilder
                .AddObjectAs<IEnumerable<Address>, Address>(root => root.Addresses, addressBuilder => addressBuilder
                    .AddArgument(argumentName, argumentValue)
                    .AddScalar(address => address.Name)
                )
            );

            Assert.Equal(expectedQueryString.Replace(" ", string.Empty), queryString.Trim().Replace(" ", string.Empty));
        }

        [Fact]
        public void BuildShouldCreateQueryStringIncludingArgumentsWithVariableValue()
        {
            var argumentName = "FakeArgumentName";
            var expectedArgumentName = $"{char.ToLowerInvariant(argumentName[0])}{argumentName.Substring(1, argumentName.Length - 1)}";
            var variableName = "FakeVariableName";
            var expectedQueryString = $"{{ addresses({expectedArgumentName}:${variableName}) {{ name }} }}";
            var queryString = GraphQueryStringBuilder.Build<QueryRoot>(rootBuilder => rootBuilder
                .AddObjectAs<IEnumerable<Address>, Address>(root => root.Addresses, addressBuilder => addressBuilder
                    .AddArgumentAsVariable(argumentName, variableName)
                    .AddScalar(address => address.Name)
                )
            );

            Assert.Equal(expectedQueryString.Replace(" ", string.Empty), queryString.Trim().Replace(" ", string.Empty));
        }

        [Fact]
        public void BuildShouldCreateQueryStringIncludingAllScalarFields()
        {
            var expectedQueryString = $"{{ addresses {{ type, name, number, otherInformation }} }}";
            var queryString = GraphQueryStringBuilder.Build<QueryRoot>(rootBuilder => rootBuilder
                .AddObjectAs<IEnumerable<Address>, Address>(root => root.Addresses, addressBuilder => addressBuilder
                    .IncludeAllScalars()
                )
            );

            Assert.Equal(expectedQueryString.Replace(" ", string.Empty), queryString.Trim().Replace(" ", string.Empty));
        }
    }
}
