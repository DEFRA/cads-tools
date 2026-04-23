#!/usr/bin/env bash
set -euo pipefail

# Resolve ROOT_DIR to the real cads-tools folder
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if [ -z "${ROOT_DIR:-}" ]; then
  export ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
fi

COMMAND="${1:-up}"

ensure_network() {
  if ! docker network inspect cads-tools >/dev/null 2>&1; then
    echo "[platform] Creating cads-tools network..."
    docker network create cads-tools
  fi
  return $?
}

if [[ "$COMMAND" == "up" ]]; then
  echo "[cads-tools] Starting infra + OIDC..."
  ensure_network
  docker compose -p cads-tools \
    -f "$ROOT_DIR/infra/docker-compose.infra.yml" \
    -f "$ROOT_DIR/oidc/docker-compose.oidc.yml" \
    up -d
  exit 0
fi

if [[ "$COMMAND" == "down" ]]; then
  echo "[cads-tools] Stopping infra + OIDC..."
  docker compose -p cads-tools \
    -f "$ROOT_DIR/infra/docker-compose.infra.yml" \
    -f "$ROOT_DIR/oidc/docker-compose.oidc.yml" \
    down -v
  exit 0
fi

echo "Unknown command: $COMMAND"
echo "Usage: run-harness.sh [up|down]"
exit 1
