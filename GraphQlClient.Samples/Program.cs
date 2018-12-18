using System;
using System.Collections.Generic;

namespace GraphQlClient.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Examples...");
            var queryString = GraphQueryStringBuilder.Build<QueryRoot>(queryBuilder => queryBuilder
                .AddObjectAs<IEnumerable<Shop>, Shop>(query => query.Shops, shopBuilder => shopBuilder
                     .AddScalar(shop => shop.Name)
                     .AddObjectAs<IEnumerable<User>, User>(shop => shop.Users, userBuilder => userBuilder
                          .IncludeAllScalars()
                          .AddObjectAs<IEnumerable<Address>,Address>(user => user.Addresses, addressBuilder => addressBuilder
                              .IncludeAllScalars()
                          )
                          .AddArgument("age", 18)
                     )
                     .AddArgumentAsVariable("name", "nameVariable")
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
