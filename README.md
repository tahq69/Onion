# Onion architecture application sample

## Details

This project is made from guide at [this site](https://www.codewithmukesh.com/blog/onion-architecture-in-aspnet-core/)
with repository pattern from [this guide](https://medium.com/@chathuranga94/generic-repository-pattern-for-asp-net-core-9e3e230e20cb)
 and JWT [from here](https://jasonwatmore.com/post/2020/05/25/aspnet-core-3-api-jwt-authentication-with-refresh-tokens)

## Migrations

Run the following commands to verify that EF Core CLI tools are correctly installed:

``` bash
dotnet restore
dotnet ef
```

### Identity database context

``` bash
# Generate migration
dotnet ef migrations add --project=Onion.Identity --startup-project=Onion.Web --context=IdentityDbContext {MigrationName}

# Update database
dotnet ef database update --project=Onion.Identity --startup-project=Onion.Web --context=IdentityDbContext
```
