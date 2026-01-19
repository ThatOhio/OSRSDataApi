# OSRSDataApi
Provides various data collection endpoints to the faux-bingo plugin.

## Local Development

This project is optimized for development using **JetBrains Rider** and **Docker Compose**.

### 1. Prerequisites
- Docker & Docker Compose
- JetBrains Rider
- A `.env` file in the root directory (copy `.env.example` to `.env` if it doesn't exist).

### 2. Running the Database (Recommended for Host Debugging)
To debug the API locally in Rider (on your host machine) while using a containerized database:

1. **Start the Database**: 
   - Open the **Docker** tool window in Rider.
   - Right-click `compose.dev.yaml` and select **Run 'compose.dev.yaml: db'**.
   - This starts Postgres and maps it to `localhost:5432`.
2. **Configure API Run Configuration**:
   - Go to **Run** -> **Edit Configurations**.
   - Select your `OSRSData.Api` profile.
   - Add an environment variable to point to the local DB:
     - `ConnectionStrings__DefaultConnection=Host=localhost;Database=OSRSData;Username=postgres;Password=postgres`
   - Set `ASPNETCORE_ENVIRONMENT=Development`.

### 3. Running the Full Stack in Docker
To run both the API and the Database in containers (closer to production):

1. Right-click `compose.yaml` in Rider and select **Edit 'Docker Compose' Configuration**.
2. In the **Compose files** field, ensure both `compose.yaml` AND `compose.dev.yaml` are selected.
3. Click **Run**.
4. The API will be available at `http://localhost:5010`.

### 4. Database Integration in Rider
You can use Rider's built-in Database tool window to inspect the containerized DB:
1. Open the **Database** tool window (usually on the right).
2. Click **+** -> **Data Source** -> **PostgreSQL**.
3. Use the following settings:
   - **Host**: `localhost`
   - **Port**: `5432`
   - **User/Password**: `postgres` / `postgres`
   - **Database**: `OSRSData`

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
