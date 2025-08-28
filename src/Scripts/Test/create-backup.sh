#!/usr/bin/env bash
set -euo pipefail

git config --global --add safe.directory "$(pwd)" >/dev/null 2>&1
PROJECT_DIR="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
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

TIMESTAMP="$(date +%Y%m%d-%H%M%S)"
BACKUP_ROOT="$PROJECT_DIR/backups/$TIMESTAMP"
mkdir -p "$BACKUP_ROOT/minio" "$BACKUP_ROOT/sql"

LOG_FILE="$BACKUP_ROOT/backup.log"
touch "$LOG_FILE"

log() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')] $*" | tee -a "$LOG_FILE"
}

MINIO_ALIAS="backup"
MINIO_URL=${MINIO_URL:-http://${MINIO_ENDPOINT:-localhost:9000}}
MINIO_ACCESS_KEY=${MINIO_ACCESS_KEY:-${MINIO_ROOT_USER:-minio}}
MINIO_SECRET_KEY=${MINIO_SECRET_KEY:-${MINIO_ROOT_PASSWORD:-minio123}}
MINIO_BUCKETS=${MINIO_BUCKETS:-quotes}

DB_HOST=${DB_HOST:-${MYSQL_HOST:-localhost}}
DB_PORT=${DB_PORT:-${MYSQL_PORT:-3306}}
DB_USER=${DB_USER:-${MYSQL_USER:-root}}
DB_PASSWORD=${DB_PASSWORD:-${MYSQL_PASSWORD:-password}}
DB_NAME=${DB_NAME:-grandlinequotes}

# Configure MinIO client and copy buckets
if command -v mc >/dev/null 2>&1; then
  log "Configurando cliente MinIO en $MINIO_URL"
  if ! mc alias set "$MINIO_ALIAS" "$MINIO_URL" "$MINIO_ACCESS_KEY" "$MINIO_SECRET_KEY" >>"$LOG_FILE" 2>&1; then
    log "No se pudo configurar el alias de MinIO"
  else
    for BUCKET in $MINIO_BUCKETS; do
      log "Copiando bucket '$BUCKET'"
      if ! mc cp --recursive "$MINIO_ALIAS/$BUCKET" "$BACKUP_ROOT/minio/$BUCKET" >>"$LOG_FILE" 2>&1; then
        log "Error al copiar el bucket '$BUCKET'"
      fi
    done
  fi
else
  log "MinIO client (mc) no encontrado, se omite la copia de buckets"
fi

# Dump SQL database
if command -v mysqldump >/dev/null 2>&1; then
  log "Exportando base de datos '$DB_NAME' desde $DB_HOST:$DB_PORT"
  if ! mysqldump --host="$DB_HOST" --port="$DB_PORT" --user="$DB_USER" --password="$DB_PASSWORD" "$DB_NAME" \
      > "$BACKUP_ROOT/sql/${DB_NAME}.sql" 2>>"$LOG_FILE"; then
    log "Error al exportar la base de datos"
  fi
else
  log "mysqldump no encontrado, se omite la exportación de la base de datos"
fi

log "Backup creado en $BACKUP_ROOT"
