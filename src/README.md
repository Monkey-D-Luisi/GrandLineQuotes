# GrandLineQuotes

GrandLineQuotes is a modular monorepo for collecting One Piece quotes. Its design makes it reusable as a template for other series or quote sources.

## Repository layout

The solution is split into four top-level folders under `src/`:

* **Back** – domain, application and infrastructure layers plus the test projects.
* **Front** – exposed services and user interfaces.
* **DevOps** – Docker configurations and deployment files.
* **Scripts** – Automation scripts for testing, backups, and database setup.

### Back-end layers

* **Domain** – Core business entities and domain logic
* **Application** – Application services and use cases
* **Infrastructure** – Database access, external service integrations
* **Client.Abstractions** – DTOs and client interfaces for API consumers

### Back-end tests

* **Tests.Core** – Shared test utilities and fixtures
* **Domain.Tests** – Unit tests for domain layer
* **Application.Tests** – Unit tests for application layer
* **Infrastructure.Tests** – Integration tests for infrastructure layer

Key components under `src/Front/`:

### REST API (`Api`)
ASP.NET Core service exposing traditional HTTP endpoints. It uses Entity Framework Core with MariaDB and a MinIO client for object storage.

### Public API (`Api.Public`)
ASP.NET Core service providing a GraphQL server with [Hot Chocolate](https://chillicream.com/docs/hotchocolate) and other public resources. It reuses the HTTP client to query the REST API.

### Administration panel (`Admin`)
MVC application for content management. It can optionally enable Google authentication and shares the same database and MinIO infrastructure as the APIs.

### HTTP Client (`Client`)
HTTP client abstraction library providing typed clients for accessing the REST API. Used by the Public API and other front-end services.

### Legal service (`Legal`)
Static service serving privacy policies (`privacy_en.html`, `privacy_es.html`) and `robots.txt` for the application.

### Client applications (`Ui`)
Kotlin/Compose client. The codebase keeps its multiplatform structure, but only the **Android** variant is currently maintained. Endpoint configuration is controlled through flavors defined in `composeApp/build.gradle.kts`. See the [module README](src/Front/Ui/README.md) for the current status and usage.

### Front-end tests

* **Api.Tests** – Integration tests for the REST API
* **Api.Public.Tests** – Tests for the GraphQL API
* **Admin.Tests** – Tests for the Admin panel
* **Client.Tests** – Tests for the HTTP client library

## Environments and Docker Compose

The project supports multiple environments:

* **Local** – For local development. Uses `docker-compose.yml` with services like `traefik`, `api-dev`, `admin-dev`, `api-public-dev`, `legal-dev`, `mariadb-dev`, `minio`, and `nginx`.
* **Sandbox** – Used by the automated test script `src/Scripts/Sandbox/run-tests.sh` for running the test suite.
* **Test** – Production-like environment with Let's Encrypt certificates, consumed by the published Android app.

### Test Environment

`src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml` defines the services used in the **Test** environment. It includes:

* **Traefik** – Reverse proxy with automatic Let's Encrypt SSL certificates
* **api**, **admin**, **public** – The three ASP.NET Core services
* **mariadb** – Database with persistent storage
* **minio** – Object storage with persistent volumes
* **legal** – Static server for privacy policies and robots.txt
* **dozzle** – Real-time log viewer for Docker containers
* **backup** – Service for creating database and MinIO backups

Environment variables are loaded from `src/DevOps/Docker-Linux/Stack/.env` (an example is provided in `.env.Example`) with environment-specific overrides in `.env.Test`. To start the Test stack:

```bash
cd src/DevOps/Docker-Linux/Stack
docker compose -f docker-compose.Test.yml --env-file .env.Test up --build -d
```

### Local Development Environment

For local development, use `docker-compose.yml`:

```bash
cd src/DevOps/Docker-Linux/Stack
docker compose up --build -d
```

This starts development versions of the services with the `-dev` suffix, accessible at `*.local` domains (e.g., `grandlinequotes-api-dev.local`, `grandlinequotes-admin-dev.local`). The environment includes:

* **traefik** – Reverse proxy for local routing
* **api-dev** – REST API development service
* **admin-dev** – Administration panel development service
* **api-public-dev** – Public GraphQL API development service
* **legal-dev** – Legal/privacy policy static server
* **mariadb-dev** – Database service
* **minio** – Object storage service
* **nginx** – Static file server for MinIO (legacy/CDN)

No SSL certificates are required for local development. The services use self-signed certificates managed by Traefik.

### Service Access

When running the Test environment with a configured domain:

* **API** – `https://api.${DOMAIN_SUFFIX}`
* **Admin Panel** – `https://admin.${DOMAIN_SUFFIX}`
* **Public GraphQL API** – `https://public.${DOMAIN_SUFFIX}/graphql`
* **MinIO Console** – `https://minio-console.${DOMAIN_SUFFIX}`
* **Log Viewer (Dozzle)** – `https://logs.${DOMAIN_SUFFIX}`
* **Traefik Dashboard** – `https://traefik.${DOMAIN_SUFFIX}` (if enabled)
* **Legal/Privacy** – `https://legal.${DOMAIN_SUFFIX}`

### Environment Variables

Copy `.env.Example` to `.env` and `.env.Test` (for the Test environment) and configure the following key variables:

**Docker & Deployment:**
* `COMPOSE_PROJECT_NAME` – Docker Compose project name (default: `grandlinequotes`)
* `DOMAIN_SUFFIX` – Your domain (e.g., `example.com`)
* `ACME_EMAIL` – Email for Let's Encrypt certificates

**Database:**
* `MYSQL_ROOT_PASSWORD` – MariaDB root password
* `MYSQL_PASSWORD` – MariaDB user password
* `MYSQL_USER` – MariaDB user (default: `grandlinequotes`)

**Object Storage:**
* `MINIO_ROOT_USER` – MinIO access key
* `MINIO_ROOT_PASSWORD` – MinIO secret key
* `MINIO_SECURE` – Use HTTPS for MinIO (true/false)
* `MINIO_PUBLIC_ENDPOINT` – Public MinIO endpoint URL
* `MINIO_PUBLIC_SECURE` – Use HTTPS for public MinIO access (true/false)

**Authentication & Security:**
* `GOOGLE_ALLOWED_EMAIL` – Email allowed for Google authentication in Admin panel (optional)
* `DOZZLE_AUTH` – Basic auth credentials for Dozzle log viewer (format: `username:password`)
* `LUCKYPENNY_LICENSE_KEY` – License key for LuckyPenny library
* `PLAYINTEGRITY_SESSION_SECRET` – Secret for Play Integrity API validation

**Mobile App Configuration:**
* `APP_PACKAGE_ID` – Android/iOS package identifier
* `APP_VERSION_NAME` – App version name
* `APP_VERSION_CODE` – App version code (integer)
* `ANDROID_VERSION_RANGE` – Supported Android API levels (e.g., `24-35`)
* `IOS_VERSION` – Target iOS version

**Privacy Policy:**
* `PRIVACY_CONTROLLER_NAME` – Data controller name
* `PRIVACY_CONTACT_EMAIL` – Contact email for privacy inquiries
* `PRIVACY_COUNTRY` – Country of the data controller
* `PRIVACY_LAST_UPDATED` – Last update date of privacy policy

See `.env.Example` for the complete list with example values.

## Running tests

The script `src/Scripts/Sandbox/run-tests.sh` prepares the environment and executes the complete test suite:

1. **Install .NET SDK 8.0** if not already present.
2. **Install and configure MariaDB** server, creating the test database and the `grandlinequotes` user with appropriate permissions.
3. **Download and launch MinIO** server, creating the `quotes` bucket for object storage testing.
4. **Install JDK 17** and the **Android command-line tools** (SDK 35/36) required by the Kotlin Multiplatform client tests.
5. **Build the .NET solution** once to avoid file locking issues.
6. **Start the API services** (REST API on port 5023, Public GraphQL API on port 5024) using the prebuilt assemblies.
7. **Run .NET tests** across all test projects without rebuilding.
8. **Run Kotlin Multiplatform tests** for the Android client (`assembleSandboxDebug` and `testSandboxDebugUnitTest`).

The script loads environment variables from `src/DevOps/Docker-Linux/Stack/.env` and, if present, `.env.$FLAVOR` (defaults to `Sandbox`) to configure database connections, MinIO endpoints, and other service settings.

To run the test suite:

```bash
bash src/Scripts/Sandbox/run-tests.sh
```

**Requirements:**
* Superuser permissions (for installing packages and starting services)
* Internet access (for downloading SDKs and dependencies)
* Ubuntu 24.04 or compatible Linux distribution

The test logs are written to `/tmp/` for debugging purposes.

## Backups

`src/Scripts/Test/` contains `create-backup.sh` and `restore-backup.sh` scripts to export and restore data from MariaDB and MinIO. Each backup is stored under `backups/<timestamp>` at the repository root together with a `backup.log` or `restore.log`. Environment variables are read from `src/DevOps/Docker-Linux/Stack/.env` and `src/DevOps/Docker-Linux/Stack/.env.Test` (use `src/DevOps/Docker-Linux/Stack/.env.Example` as a template).

To create a backup within the Docker network:

```bash
docker compose -f src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml run --rm backup
```

Or run the script directly:

```bash
bash src/Scripts/Test/create-backup.sh
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

## Deployment

The `src/Scripts/Test/deploy-test.sh` script automates deployment to the Test environment:

1. Pulls latest code from the specified branch (defaults to `master`)
2. Loads environment variables from `src/DevOps/Docker-Linux/Stack/.env` and `.env.Test`
3. Configures firewall rules (opens ports 80 and 443 using UFW)
4. Validates Docker Compose configuration
5. Pulls Docker images and builds services
6. Starts all services with `--remove-orphans` flag
7. Performs health checks on all endpoints (api, admin, public, minio, minio-console, logs, legal)

Run the deployment script:

```bash
bash src/Scripts/Test/deploy-test.sh
```

**Environment variables:**
* `BRANCH` – Git branch to deploy (default: `master`)

**Requirements:**
* Docker and Docker Compose plugin installed
* Sudo permissions for firewall configuration
* Git repository with remote access
* Configured `.env` and `.env.Test` files with required variables

This script is designed to run on a production server and includes safety checks to validate configuration before deployment.

## Database Schema

SQL table creation scripts are available in `src/Scripts/Tables/`:

* `character.sql` – Character information
* `quote.sql` – Quote data
* `quote_translation.sql` – Quote translations
* `episode.sql` – Episode information
* `episode_title.sql` – Episode titles in multiple languages
* `arc.sql` – Story arc information
* `arc_title.sql` – Arc titles in multiple languages
* `saga.sql` – Saga information
* `saga_title.sql` – Saga titles in multiple languages

These scripts define the complete database schema used by the application.
