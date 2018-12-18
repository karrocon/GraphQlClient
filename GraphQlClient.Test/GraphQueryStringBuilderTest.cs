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
            var expectedQueryString = $"{{ Addresses({argumentName}:\"{argumentValue}\") {{ Name }} }}";
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
            var variableName = "FakeVariableName";
            var expectedQueryString = $"{{ Addresses({argumentName}:${variableName}) {{ Name }} }}";
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
            var expectedQueryString = $"{{ Addresses {{ Type, Name, Number, OtherInformation }} }}";
            var queryString = GraphQueryStringBuilder.Build<QueryRoot>(rootBuilder => rootBuilder
                .AddObjectAs<IEnumerable<Address>, Address>(root => root.Addresses, addressBuilder => addressBuilder
                    .IncludeAllScalars()
                )
            );

            Assert.Equal(expectedQueryString.Replace(" ", string.Empty), queryString.Trim().Replace(" ", string.Empty));
        }
    }
}
