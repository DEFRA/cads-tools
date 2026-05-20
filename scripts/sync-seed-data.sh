#!/usr/bin/env bash
set -euo pipefail

SEED_PATH="../cads-data-seed/sql"
BUCKET="cads-internal-bucket"
echo "[cads-tools] Syncing SQL seed data into LocalStack S3..."
AWS_REQUEST_CHECKSUM_CALCULATION=when_required \
AWS_RESPONSE_CHECKSUM_VALIDATION=when_required \
aws --endpoint-url=http://localhost:4566 \
    s3 sync "$SEED_PATH" "s3://$BUCKET/sql" \
    --delete \
    --no-sign-request
echo "[cads-tools] SQL seed data sync complete."