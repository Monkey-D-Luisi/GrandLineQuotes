#!/usr/bin/env bash
set -euo pipefail

# Usage: restore-backup.sh <backup-folder>
# If no folder is provided, the latest backup is used.

git config --global --add safe.directory "$(pwd)" >/dev/null 2>&1
PROJECT_DIR="$(git rev-parse --show-toplevel 2>/dev/null || echo /srv/GrandLineQuotes)"
cd "$PROJECT_DIR"

ENV_BASE="src/DevOps/Docker-Linux/Stack/.env"
ENV_OVR="src/DevOps/Docker-Linux/Stack/.env.Test"

set +u
set -a

load_env_file() {
  local file="$1"
  if [ -f "$file" ]; then
    local tmp
    tmp="$(mktemp)"
    sed 's/\$\$/\\$/g' "$file" >"$tmp"
    # shellcheck disable=SC1090
    . "$tmp"
    rm "$tmp"
  fi
}

load_env_file "$ENV_BASE"
load_env_file "$ENV_OVR"

set +a
set -u

BACKUP_ROOT="$PROJECT_DIR/backups"
TARGET="${1:-latest}"

if [[ "$TARGET" == "latest" ]]; then
  TARGET="$(ls -1 "$BACKUP_ROOT" | sort | tail -n 1)"
fi

BACKUP_DIR="$BACKUP_ROOT/$TARGET"

if [[ ! -d "$BACKUP_DIR" ]]; then
  echo "Backup '$TARGET' no encontrado" >&2
  exit 1
fi

LOG_FILE="$BACKUP_DIR/restore.log"
touch "$LOG_FILE"

log() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')] $*" | tee -a "$LOG_FILE"
}

MINIO_ALIAS="backup"
MINIO_URL=${MINIO_URL:-http://${MINIO_ENDPOINT:-localhost:9000}}
MINIO_ACCESS_KEY=${MINIO_ACCESS_KEY:-${MINIO_ROOT_USER:-minio}}
MINIO_SECRET_KEY=${MINIO_SECRET_KEY:-${MINIO_ROOT_PASSWORD:-minio123}}

DB_HOST=${DB_HOST:-${MYSQL_HOST:-localhost}}
DB_PORT=${DB_PORT:-${MYSQL_PORT:-3306}}
DB_USER=${DB_USER:-${MYSQL_USER:-root}}
DB_PASSWORD=${DB_PASSWORD:-${MYSQL_PASSWORD:-password}}
DB_NAME=${DB_NAME:-grandlinequotes}

# Restore MinIO buckets
if command -v mc >/dev/null 2>&1; then
  log "Configurando cliente MinIO en $MINIO_URL"
  if ! mc alias set "$MINIO_ALIAS" "$MINIO_URL" "$MINIO_ACCESS_KEY" "$MINIO_SECRET_KEY" >>"$LOG_FILE" 2>&1; then
    log "No se pudo configurar el alias de MinIO"
  else
    for BUCKET_DIR in "$BACKUP_DIR/minio"/*; do
      [ -d "$BUCKET_DIR" ] || continue
      BUCKET_NAME="$(basename "$BUCKET_DIR")"
      log "Restaurando bucket '$BUCKET_NAME'"
      mc mb "$MINIO_ALIAS/$BUCKET_NAME" >>"$LOG_FILE" 2>&1 || true
      if ! mc cp --recursive "$BUCKET_DIR" "$MINIO_ALIAS/$BUCKET_NAME" >>"$LOG_FILE" 2>&1; then
        log "Error al restaurar el bucket '$BUCKET_NAME'"
      fi
    done
  fi
else
  log "MinIO client (mc) no encontrado, se omite la restauración de buckets"
fi

# Restore SQL database
if command -v mysql >/dev/null 2>&1 && [[ -f "$BACKUP_DIR/sql/${DB_NAME}.sql" ]]; then
  log "Importando base de datos '$DB_NAME' en $DB_HOST:$DB_PORT"
  if ! mysql --host="$DB_HOST" --port="$DB_PORT" --user="$DB_USER" --password="$DB_PASSWORD" "$DB_NAME" < "$BACKUP_DIR/sql/${DB_NAME}.sql" >>"$LOG_FILE" 2>&1; then
    log "Error al importar la base de datos"
  fi
else
  log "mysql no encontrado o fichero de backup inexistente"
fi

log "Restaurado backup desde $BACKUP_DIR"

