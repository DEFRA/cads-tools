#!/usr/bin/env bash
set -euo pipefail

sed -i 's/\r$//' "$0"

SEED_PATH="../cads-data-seed/sql"
BUCKET="cads-internal-bucket"
ENDPOINT="http://localhost:4566"
REGION="eu-west-2"

export AWS_REGION="$REGION"
export AWS_DEFAULT_REGION="$REGION"
export AWS_ACCESS_KEY_ID="test"
export AWS_SECRET_ACCESS_KEY="test"
export AWS_EC2_METADATA_DISABLED=true

echo "[cads-tools] Syncing SQL seed data into LocalStack S3..."

# Wait for bucket to exist
echo "[cads-tools] Waiting for bucket '$BUCKET'..."
for i in {1..30}; do
  if aws --endpoint-url="$ENDPOINT" s3api head-bucket --bucket "$BUCKET" >/dev/null 2>&1; then
    echo "[cads-tools] Bucket exists."
    break
  fi
  echo "[cads-tools] Bucket not ready... retrying ($i/30)"
  sleep 1
done

AWS_REQUEST_CHECKSUM_CALCULATION=when_required \
AWS_RESPONSE_CHECKSUM_VALIDATION=when_required \
aws --endpoint-url="$ENDPOINT" s3 sync "$SEED_PATH" "s3://$BUCKET/sql" \
    --delete \
    --no-sign-request
echo "[cads-tools] SQL seed data sync complete."