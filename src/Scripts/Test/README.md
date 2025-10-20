# Test Environment Scripts

Utility scripts for managing the Test (production-like) environment, including backup/restore operations and automated deployment.

## Available Scripts

### `create-backup.sh`

Creates a complete backup of the Test environment data, including:
* **MariaDB database** – Full SQL dump of the `grandlinequotes` database
* **MinIO buckets** – All objects from configured buckets (default: `quotes`)

**Features:**
* Automatically loads environment variables from `src/DevOps/Docker-Linux/Stack/.env` and `.env.Test`
* Creates timestamped backup directory at `backups/<timestamp>/` in the repository root
* Generates detailed `backup.log` for troubleshooting
* Safely handles special characters in environment variable values

**Usage:**

Within the Docker network (recommended):
```bash
docker compose -f src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml run --rm backup
```

Directly on the host:
```bash
bash src/Scripts/Test/create-backup.sh
```

**Requirements:**
* `mysqldump` command-line tool
* MinIO client (`mc`) for object storage backup

### `restore-backup.sh`

Restores data from a previous backup, recreating the database and MinIO bucket contents.

**Features:**
* Automatically selects the most recent backup if no directory is specified
* Loads environment variables from `src/DevOps/Docker-Linux/Stack/.env` and `.env.Test`
* Generates detailed `restore.log` in the backup directory
* Creates MinIO buckets if they don't exist

**Usage:**

Restore the latest backup:
```bash
bash src/Scripts/Test/restore-backup.sh
```

Restore a specific backup:
```bash
bash src/Scripts/Test/restore-backup.sh backups/20250120-143000
```

**Requirements:**
* `mysql` command-line tool
* MinIO client (`mc`) for object storage restoration

### `deploy-test.sh`

Automates the deployment process for the Test environment with safety checks and validation.

**Features:**
* Pulls latest code from the specified branch (default: `master`)
* Loads and validates environment variables
* Configures firewall rules (opens ports 80 and 443)
* Validates Docker Compose configuration before deployment
* Builds and starts all services with zero-downtime deployment
* Performs health checks on all endpoints (api, admin, public, minio, logs, legal)

**Usage:**

```bash
bash src/Scripts/Test/deploy-test.sh
```

**Environment Variables:**
* `BRANCH` – Git branch to deploy (default: `master`)
* `DOMAIN_SUFFIX` – Required domain for the deployment
* `ACME_EMAIL` – Required email for Let's Encrypt certificates
* Additional variables from `.env` and `.env.Test`

**Requirements:**
* `docker` and Docker Compose plugin
* `sudo` permissions for firewall configuration
* Git repository with remote access

## Environment Configuration

All scripts load environment variables from:
1. `src/DevOps/Docker-Linux/Stack/.env` – Base configuration
2. `src/DevOps/Docker-Linux/Stack/.env.Test` – Test environment overrides

Use `src/DevOps/Docker-Linux/Stack/.env.Example` as a template. The `.env.Test` file should contain production-specific values and is not version-controlled for security reasons.

## Backup Directory Structure

```
backups/
└── YYYYMMDD-HHMMSS/
    ├── backup.log (or restore.log)
    ├── sql/
    │   └── grandlinequotes.sql
    └── minio/
        └── quotes/
            └── [object files]
```

## Notes

* Backup and restore operations can be run on the host machine or within Docker containers
* The scripts handle environment variable substitution safely, including special characters like `$`
* Always test restore operations in a non-production environment first
* Keep regular backups before performing major updates or deployments
