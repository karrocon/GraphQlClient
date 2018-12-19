# GraphQlClient

Simple GraphQL client wrapper for .NET.

## Installation

The packages are available at [nuget.org](https://www.nuget.org/) for installation (see [karrocon.GraphQlClient](https://www.nuget.org/packages/karrocon.GraphQlClient/) and [karrocon.GraphQlClient.Relay](https://www.nuget.org/packages/karrocon.GraphQlClient.Relay/)).

## Examples

### Build query strings easily

Supports query string building in an expression-based style:

```
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
```

#### Relay

It also supports building query strings for Relay-based servers:

```
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
```

### Query all pages at once using Relay

The library features an extension method for graphql-dotnet.IGraphQlClient to query every Relay page at once:

```
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

var result = await client.QueryAllPagesAsync(new GraphQlRequest { Query = queryString });
```
