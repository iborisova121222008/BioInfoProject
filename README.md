# BIP06 Biomedical Data Management Tool

University software project:

**Design and Implementation of a Software Tool for Management of Large Biomedical Data (BIP06)**

This is a local ASP.NET Core and Blazor application for importing, storing, filtering, analyzing, visualizing, and exporting biomedical epidemiological CSV datasets.

## Technology Stack

- Backend: ASP.NET Core Web API
- Frontend: Blazor WebAssembly hosted by ASP.NET Core
- Database: SQLite
- ORM: Entity Framework Core
- CSV processing: CsvHelper
- UI: Bootstrap
- Charts: Chart.js

No Docker is required.

## Project Structure

```text
BioInfoProject.sln
global.json
README.md
SampleDatasets/
src/
  BioInfoProject.Client/   Blazor frontend
  BioInfoProject.Server/   ASP.NET Core Web API backend
  BioInfoProject.Shared/   Shared DTOs and request models
```

## Supported CSV Structure

The importer expects comma-delimited CSV files with these columns:

```text
date_reported
country_code
country
who_region
new_cases
cumulative_cases
new_deaths
cumulative_deaths
```

Column matching is case-insensitive, so files using names like `Date_reported` are also accepted.

Each uploaded CSV file is stored as a separate dataset in the database. The dashboard and data browser always analyze the selected dataset.

## Sample Datasets

Sample CSV files are included in:

```text
SampleDatasets/
```

Files:

- `WHO_COVID_Global_Data.csv`
- `Europe_COVID_Test_Data.csv`

These datasets contain different countries, WHO regions, dates, cases, cumulative cases, deaths, and cumulative deaths so that filters, tables, statistics, and charts show meaningful differences.

## Database

SQLite database location:

```text
src/BioInfoProject.Server/Data/bioinfo.db
```

Main tables:

- `Datasets`
- `EpidemiologicalRecords`

Entity Framework migrations are stored in:

```text
src/BioInfoProject.Server/Migrations/
```

## Running the Application

From the project root:

```powershell
dotnet restore BioInfoProject.sln
dotnet build BioInfoProject.sln
dotnet run --project src\BioInfoProject.Server\BioInfoProject.Server.csproj --urls http://localhost:5293
```

Open:

```text
http://localhost:5293
```

Useful pages:

```text
http://localhost:5293/import
http://localhost:5293/data
```

Swagger API documentation is available in development:

```text
http://localhost:5293/swagger
```

## Main Workflow

1. Open the Import CSV page.
2. Upload a CSV file from `SampleDatasets/` or another compatible biomedical time-series CSV.
3. The system validates the required columns.
4. The system cleans and imports valid rows.
5. A new dataset record is created for that file.
6. Open the Dashboard and choose a dataset.
7. Use country and WHO region filters.
8. Open the Data Browser for searching, sorting, pagination, date filtering, and export.

## Implemented Features

- CSV upload and import
- Required-column validation
- Data cleaning and preprocessing
- Separate dataset metadata for each upload
- SQLite relational persistence
- Dataset selection
- Country and WHO region filtering
- Date range filtering
- Search
- Sorting
- Pagination
- Dashboard statistics
- Cases and deaths over time
- Country summaries
- WHO region summaries
- Chart.js visualizations
- Filtered CSV export

## API Endpoints

Import:

```text
POST /api/import
```

Datasets:

```text
GET /api/datasets
GET /api/datasets/{id}
```

Records:

```text
POST /api/records/filter
GET /api/records/countries/{datasetId}
GET /api/records/regions/{datasetId}
```

Analytics:

```text
POST /api/analytics/dashboard
POST /api/analytics/time-series
POST /api/analytics/by-country
POST /api/analytics/by-region
```

Export:

```text
POST /api/export/records
```

## Notes

Chart.js is loaded from a CDN in the Blazor client. If the machine has no internet access, the rest of the application still works, but dashboard charts will not render until Chart.js is available.

If you want to start with an empty database, stop the application and delete:

```text
src/BioInfoProject.Server/Data/bioinfo.db
src/BioInfoProject.Server/Data/bioinfo.db-shm
src/BioInfoProject.Server/Data/bioinfo.db-wal
```

Then recreate the database:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update --project src\BioInfoProject.Server\BioInfoProject.Server.csproj --startup-project src\BioInfoProject.Server\BioInfoProject.Server.csproj
```
