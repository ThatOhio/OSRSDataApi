# OSRSDataApi
Provides various data collection endpoints to the faux-bingo plugin.

## Development

### Database Migrations

This project uses Entity Framework Core for database management. Due to environment constraints where the `Microsoft.AspNetCore.App` runtime might not be available to the `dotnet ef` tool, migrations should be managed using the `OSRSData.DAL` project as both the target and the startup project.

A `IDesignTimeDbContextFactory` is provided in `OSRSData.DAL` to facilitate this.

#### Adding a Migration

To add a new migration, run the following command from the project root:

```bash
dotnet ef migrations add <MigrationName> --project OSRSData.DAL --startup-project OSRSData.DAL
```

#### Removing a Migration

To remove the last migration:

```bash
dotnet ef migrations remove --project OSRSData.DAL --startup-project OSRSData.DAL
```

#### Applying Migrations

Migrations are automatically applied by the API on startup in `Program.cs`. However, if you need to apply them manually to a local database:

```bash
dotnet ef database update --project OSRSData.DAL --startup-project OSRSData.DAL
```

Note: Ensure the database is running (e.g., via `docker compose up db`) before applying migrations manually.
