# Migration tool usage

## Add migration

`dotnet ef migrations add <MigrationName> -n VoteUp.PortalData.Migrations`

## Update database

`dotnet ef database update --connection "Host=<host>;Port=<port>;Database=<database>;Username=<username>;Password=<password>"`