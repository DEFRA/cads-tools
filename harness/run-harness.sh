#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
export ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

COMMAND="${1:-up}"

if [[ "$COMMAND" == "up" ]]; then
  echo "[cads-tools] Starting infra + OIDC..."
  docker compose \
    -f "$ROOT_DIR/infra/docker-compose.infra.yml" \
    -f "$ROOT_DIR/oidc/docker-compose.oidc.yml" \
    up -d
  exit 0
fi

if [[ "$COMMAND" == "down" ]]; then
  echo "[cads-tools] Stopping infra + OIDC..."
  docker compose \
    -f "$ROOT_DIR/infra/docker-compose.infra.yml" \
    -f "$ROOT_DIR/oidc/docker-compose.oidc.yml" \
    down -v
  exit 0
fi

echo "Unknown command: $COMMAND"
echo "Usage: run-harness.sh [up|down]"
exit 1