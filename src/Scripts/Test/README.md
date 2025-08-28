# Test Scripts

Utilities for the Test environment.

## Backups

`create-backup.sh` and `restore-backup.sh` export and restore data from MariaDB and MinIO, storing the results in `backups/<timestamp>` at the repository root. Each run produces a `backup.log` or `restore.log` detailing the operation. The scripts automatically load variables from `src/DevOps/Docker-Linux/Stack/.env` and `.env.Test` (not versioned; use `.env.Example` as a template).

### Usage

To create a backup within the internal Docker network:

```bash
docker compose -f src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml run --rm backup
```

To restore an existing backup (defaults to the most recent):

```bash
bash restore-backup.sh [directory]
```
