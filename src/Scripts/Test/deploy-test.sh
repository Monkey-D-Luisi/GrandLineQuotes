#!/usr/bin/env bash
set -Eeuo pipefail

# --- Config ---
BRANCH="${BRANCH:-master}"
COMPOSE_FILE="src/DevOps/Docker-Linux/Stack/docker-compose.Test.yml"
ENV_BASE="src/DevOps/Docker-Linux/Stack/.env"
ENV_OVR="src/DevOps/Docker-Linux/Stack/.env.Test"
PROJECT_DIR="$(git rev-parse --show-toplevel 2>/dev/null || echo /srv/GrandLineQuotes)"

# --- Precondiciones ---
command -v docker >/dev/null || { echo "Docker no está instalado"; exit 1; }
docker compose version >/dev/null || { echo "Falta el plugin docker compose"; exit 1; }

cd "$PROJECT_DIR"

# --- Git ---
git fetch --prune origin
git reset --hard "origin/${BRANCH}"

# --- Cargar .env y .env.Test al entorno (OVR pisa BASE) ---
# Evita problemas con set -u mientras hacemos 'source'
set +u
set -a

load_env_file() {
  local file="$1"
  if [ -f "$file" ]; then
    local tmp
    tmp="$(mktemp)"
    # Sustituye pares de '$$' por '\$' para evitar la expansión a PID
    sed 's/\$\$/\\$/g' "$file" >"$tmp"
    echo "Cargando variables desde $file"
    # shellcheck disable=SC1090
    . "$tmp"
    rm "$tmp"
  else
    echo "Aviso: archivo de entorno $file no encontrado"
  fi
}

load_env_file "$ENV_BASE"
load_env_file "$ENV_OVR"

set +a
set -u

echo "Variables de entorno cargadas:" >&2
echo "  DOMAIN_SUFFIX=${DOMAIN_SUFFIX:-<no definido>}" >&2
echo "  ACME_EMAIL=${ACME_EMAIL:-<no definido>}" >&2
[ -n "${DOZZLE_AUTH:-}" ] && echo "  DOZZLE_AUTH longitud=${#DOZZLE_AUTH}" >&2

# --- Firewall ---
sudo ufw allow 80,443/tcp >/dev/null || true

# Comprobaciones mínimas ya con el entorno cargado
: "${DOMAIN_SUFFIX:?Falta DOMAIN_SUFFIX en .env / .env.Test}"
: "${ACME_EMAIL:?Falta ACME_EMAIL en .env / .env.Test}"

# --- Validación de compose ---
docker compose \
  -f "$COMPOSE_FILE" \
  --project-directory "$PROJECT_DIR" \
  config >/dev/null

# --- Pull / Build / Up ---
docker compose \
  -f "$COMPOSE_FILE" \
  --project-directory "$PROJECT_DIR" \
  pull

docker compose \
  -f "$COMPOSE_FILE" \
  --project-directory "$PROJECT_DIR" \
  up -d --build --remove-orphans

# --- Probing rápido ---
for h in api admin public minio minio-console logs legal; do
  echo -n "Probing $h.$DOMAIN_SUFFIX ... "
  curl -sS -m 5 -H "Host: $h.$DOMAIN_SUFFIX" http://127.0.0.1/ >/dev/null && echo "OK" || echo "FALLÓ"
done

echo "Despliegue completado."
