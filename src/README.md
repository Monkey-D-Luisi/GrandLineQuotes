# GrandLineQuotes

GrandLineQuotes is a modular monorepo for collecting One Piece quotes. Its design makes it reusable as a template for other series or quote sources.

## Repository layout

The solution is split into two top-level folders under `src/`:

* **Back** – domain, application and infrastructure layers plus the test projects.
* **Front** – exposed services and user interfaces.

Key components under `src/Front/`:

### REST API (`Api`)
ASP.NET Core service exposing traditional HTTP endpoints. It uses Entity Framework Core with MariaDB and a MinIO client for object storage.

### Public API (`Api.Public`)
ASP.NET Core service providing a GraphQL server with [Hot Chocolate](https://chillicream.com/docs/hotchocolate) and other public resources. It reuses the HTTP client to query the REST API.

### Administration panel (`Admin`)
MVC application for content management. It can optionally enable Google authentication and shares the same database and MinIO infrastructure as the APIs.

### Client applications (`Ui`)
Kotlin/Compose client. The codebase keeps its multiplatform structure, but only the **Android** variant is currently maintained. Endpoint configuration is controlled through flavors defined in `composeApp/build.gradle.kts`. See the [module README](src/Front/Ui/README.md) for the current status and usage.

## Docker Compose environment

`src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml` defines the services used in the **Test** environment, the most complete one and the one consumed by the Android app. It includes Traefik as a reverse proxy, the APIs, MariaDB and MinIO with persistent volumes.

Shared credentials are taken from `.env` (an example is provided in `.env.Example`) and environment-specific values live in `.env.<Environment>`. To start the Test stack:

```bash
cd src/DevOps/Docker-Linux/Stack
docker compose -f docker-compose.Test.yml --env-file .env.Test up --build -d
```

## Running tests

The script `src/Scripts/Sandbox/run-tests.sh` prepares the environment and executes the test suite:

1. Install **.NET SDK 8.0**.
2. Install and configure **MariaDB**, creating the test database and the `grandlinequotes` user.
3. Download and launch **MinIO**, creating the `quotes` bucket.
4. Install **JDK 17** and the **Android** command-line tools required by the multiplatform client tests.
5. Build the code, start the APIs temporarily and run both the .NET and Kotlin tests.

The script loads `.env` and, if present, `.env.$FLAVOR` (defaults to `Sandbox`) to supply environment data. Run it with:

```bash
bash src/Scripts/Sandbox/run-tests.sh
```

Superuser permissions and internet access are required.

## Backups

`src/Scripts/Test/` contains scripts to export and restore data from MariaDB and MinIO. Each backup is stored under `backups/<timestamp>` at the repository root together with a `backup.log` or `restore.log`. Environment variables are read from `.env` and `.env.Test` (use `src/DevOps/Docker-Linux/Stack/.env.Example` as a template).

To create a backup within the Docker network:

```bash
docker compose -f src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml run --rm backup
```

To restore the latest backup:

```bash
bash src/Scripts/Test/restore-backup.sh
```

A specific directory can also be provided:

```bash
bash src/Scripts/Test/restore-backup.sh backups/<timestamp>
```

The scripts require `mysqldump`, `mysql` and the MinIO client `mc`.
