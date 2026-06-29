#!/usr/bin/env bash
set -euo pipefail

# Resolve ROOT_DIR to the real cads-tools folder
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if [ -z "${ROOT_DIR:-}" ]; then
  export ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
fi

COMMAND="${1:-up}"

ensure_network() {
  if ! docker network inspect cads-network >/dev/null 2>&1; then
    echo "[platform] Creating cads network..."
    docker network create cads-network
  fi
  return $?
}

if [[ "$COMMAND" == "up" ]]; then
  echo "[cads-tools] Starting infra + OIDC..."
  ensure_network
  docker compose -p cads \
    -f "$ROOT_DIR/infra/docker-compose.infra.yml" \
    -f "$ROOT_DIR/oidc/docker-compose.oidc.yml" \
    up -d
  echo "[cads-tools] Waiting for LocalStack to be ready..."
  until curl -s http://localhost:4566/_localstack/health | grep '"s3": "running"' >/dev/null; do
    sleep 1
  done
  echo "[cads-tools] LocalStack is ready..."
  if [[ "${2:-}" == "--sync-data-seed" ]]; then
    echo "[cads-tools] Syncing SQL seed data to S3..."
    "$ROOT_DIR/scripts/sync-seed-data.sh"
    echo "[cads-tools] SQL seed data has been synced to S3..."
  fi
  exit 0
fi

if [[ "$COMMAND" == "down" ]]; then
  echo "[cads-tools] Stopping infra + OIDC..."
  VOLUMES_FLAG=""
  if [[ "${2:-}" == "--clean" ]]; then
    VOLUMES_FLAG="-v"
    echo "[cads-tools] --clean specified, volumes will be removed..."
  fi
  docker compose -p cads \
    -f "$ROOT_DIR/infra/docker-compose.infra.yml" \
    -f "$ROOT_DIR/oidc/docker-compose.oidc.yml" \
    down $VOLUMES_FLAG
  exit 0
fi

echo "Unknown command: $COMMAND"
echo "Usage: run-harness.sh [up|down]"
exit 1
