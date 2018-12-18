using GraphQlClient.Relay.Extensions;
using System;

namespace GraphQlClient.Relay.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Examples...");
            var queryString = GraphQueryStringBuilder.Build<QueryRoot>(queryBuilder => queryBuilder
                .AddConnection(query => query.Shops, 10, shopBuilder => shopBuilder
                    .AddScalar(shop => shop.Name)
                    .AddConnection(shop => shop.Users, 5, userBuilder => userBuilder
                        .IncludeAllScalars()
                        .AddConnection(user => user.Addresses, 1, addressBuilder => addressBuilder
                            .IncludeAllScalars()
                        )
                    )
                )
            );

            Console.WriteLine(queryString);
            Console.ReadLine();

            var parsedQuery = GraphQueryStringParser.Parse(queryString);

            Console.WriteLine(((IGraphQueryable)parsedQuery).ToQueryString());
            Console.ReadLine();
        }
    }
}
