#!/bin/bash
sed -i 's/\r$//' "$0"

export AWS_REGION=eu-west-2
export AWS_DEFAULT_REGION=eu-west-2
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test

set -e

# S3
echo "Creating S3 resources..."

## Create External Bucket
EXTERNAL_BUCKET_NAME="cads-external-bucket"

existing_external_bucket=$(awslocal s3api list-buckets \
  --query "Buckets[?Name=='$EXTERNAL_BUCKET_NAME'].Name" \
  --output text)

if [ "$existing_external_bucket" == "$EXTERNAL_BUCKET_NAME" ]; then
  echo "S3 bucket already exists: $EXTERNAL_BUCKET_NAME"
else
  awslocal s3api create-bucket \
    --bucket "$EXTERNAL_BUCKET_NAME" \
    --region eu-west-2 \
    --create-bucket-configuration LocationConstraint=eu-west-2
  echo "S3 bucket created: $EXTERNAL_BUCKET_NAME"
fi

## Create Internal Bucket
INTERNAL_BUCKET_NAME="cads-internal-bucket"

existing_internal_bucket=$(awslocal s3api list-buckets \
  --query "Buckets[?Name=='$INTERNAL_BUCKET_NAME'].Name" \
  --output text)

if [ "$existing_internal_bucket" == "$INTERNAL_BUCKET_NAME" ]; then
  echo "S3 bucket already exists: $INTERNAL_BUCKET_NAME"
else
  awslocal s3api create-bucket \
    --bucket "$INTERNAL_BUCKET_NAME" \
    --region eu-west-2 \
    --create-bucket-configuration LocationConstraint=eu-west-2
  echo "S3 bucket created: $INTERNAL_BUCKET_NAME"
fi

## Create Reports Bucket (represents the external CCoE-owned bucket, accessed via IAM role in real environments)
REPORTS_BUCKET_NAME="cads-reports-bucket"
REPORTS_KMS_ALIAS="alias/cads-reports-key"

### Create (or reuse) the KMS key used for SSE-KMS encryption of uploaded reports
existing_reports_kms_alias=$(awslocal kms list-aliases \
  --query "Aliases[?AliasName=='$REPORTS_KMS_ALIAS'].AliasName" \
  --output text)

if [ "$existing_reports_kms_alias" == "$REPORTS_KMS_ALIAS" ]; then
  echo "KMS alias already exists: $REPORTS_KMS_ALIAS"
else
  reports_kms_key_id=$(awslocal kms create-key \
    --description "Reports bucket encryption key" \
    --output text \
    --query 'KeyMetadata.KeyId')
  echo "KMS key created: $reports_kms_key_id"

  awslocal kms create-alias \
    --alias-name "$REPORTS_KMS_ALIAS" \
    --target-key-id "$reports_kms_key_id"
  echo "KMS alias created: $REPORTS_KMS_ALIAS"
fi

REPORTS_KMS_KEY_ARN="arn:aws:kms:eu-west-2:000000000000:$REPORTS_KMS_ALIAS"
echo "Reports KMS key ARN: $REPORTS_KMS_KEY_ARN"

### Create the bucket
existing_reports_bucket=$(awslocal s3api list-buckets \
  --query "Buckets[?Name=='$REPORTS_BUCKET_NAME'].Name" \
  --output text)

if [ "$existing_reports_bucket" == "$REPORTS_BUCKET_NAME" ]; then
  echo "S3 bucket already exists: $REPORTS_BUCKET_NAME"
else
  awslocal s3api create-bucket \
    --bucket "$REPORTS_BUCKET_NAME" \
    --region eu-west-2 \
    --create-bucket-configuration LocationConstraint=eu-west-2
  echo "S3 bucket created: $REPORTS_BUCKET_NAME"
fi

### Configure default SSE-KMS encryption on the bucket, mirroring the real CCoE bucket's
### encryption-at-rest requirement. CadsCds also sets per-object SSE-KMS params on upload,
### so this is a belt-and-braces default for anything uploaded without explicit params.
echo "Setting default SSE-KMS encryption for: $REPORTS_BUCKET_NAME"
awslocal s3api put-bucket-encryption \
  --bucket "$REPORTS_BUCKET_NAME" \
  --server-side-encryption-configuration "{
    \"Rules\": [
      {
        \"ApplyServerSideEncryptionByDefault\": {
          \"SSEAlgorithm\": \"aws:kms\",
          \"KMSMasterKeyID\": \"$REPORTS_KMS_KEY_ARN\"
        },
        \"BucketKeyEnabled\": true
      }
    ]
  }"

# SQS
echo "Creating SQS resources..."

## 'cads-cds-queue'

### Create 'cads-cds-queue.fifo' DLQ
dlq_url=$(awslocal sqs create-queue \
  --queue-name cads-cds-queue-deadletter.fifo \
  --attributes '{"FifoQueue":"true","ContentBasedDeduplication":"true"}' \
  --output text \
  --query 'QueueUrl')
echo "'cads-cds-queue.fifo' DLQ created: $dlq_url"

### Get the ARN of the 'cads-cds-queue.fifo' DLQ, which is needed for the redrive policy
dlq_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$dlq_url" \
  --attribute-names QueueArn \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-cds-queue.fifo' DLQ ARN: $dlq_arn"

### Create 'cads-cds-queue.fifo' queue
queue_url=$(awslocal sqs create-queue \
  --queue-name cads-cds-queue.fifo \
  --attributes '{"FifoQueue":"true","ContentBasedDeduplication":"true"}' \
  --output text \
  --query 'QueueUrl')
echo "'cads-cds-queue.fifo' queue created: $queue_url"

### Define the Redrive Policy, linking the main queue to the DLQ.
redrive_policy_json=$(cat <<EOF
{
  "deadLetterTargetArn": "$dlq_arn",
  "maxReceiveCount": "3"
}
EOF
)

### Set redrive policy for 'cads-cds-queue.fifo' DLQ
echo "Set redrive policy for 'cads-cds-queue.fifo' DLQ..."
awslocal sqs set-queue-attributes \
  --queue-url "$queue_url" \
  --attributes "{\"RedrivePolicy\":\"{\\\"deadLetterTargetArn\\\":\\\"$dlq_arn\\\",\\\"maxReceiveCount\\\":\\\"3\\\"}\"}"

### Get the 'cads-cds-queue.fifo' queue ARN
queue_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$queue_url" \
  --attribute-name QueueArn \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-cds-queue.fifo' queue ARN: $queue_arn"

## 'cads-bridge-queue.fifo'

### Create 'cads-bridge-queue.fifo' DLQ
dlq_url=$(awslocal sqs create-queue \
  --queue-name cads-bridge-queue-deadletter.fifo \
  --attributes '{"FifoQueue":"true","ContentBasedDeduplication":"true"}' \
  --output text \
  --query 'QueueUrl')
echo "'cads-bridge-queue.fifo' DLQ created: $dlq_url"

### Get the ARN of the 'cads-bridge-queue.fifo' DLQ, which is needed for the redrive policy
dlq_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$dlq_url" \
  --attribute-names QueueArn \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-bridge-queue.fifo' DLQ ARN: $dlq_arn"

### Create 'cads-bridge-queue.fifo' queue
queue_url=$(awslocal sqs create-queue \
  --queue-name cads-bridge-queue.fifo \
  --attributes '{"FifoQueue":"true","ContentBasedDeduplication":"true"}' \
  --output text \
  --query 'QueueUrl')
echo "'cads-bridge-queue.fifo' queue created: $queue_url"

### Define the Redrive Policy, linking the main queue to the DLQ.
redrive_policy_json=$(cat <<EOF
{
  "deadLetterTargetArn": "$dlq_arn",
  "maxReceiveCount": "3"
}
EOF
)

### Set redrive policy for 'cads-bridge-queue.fifo' DLQ
echo "Set redrive policy for 'cads-bridge-queue.fifo' DLQ..."
awslocal sqs set-queue-attributes \
  --queue-url "$queue_url" \
  --attributes "{\"RedrivePolicy\":\"{\\\"deadLetterTargetArn\\\":\\\"$dlq_arn\\\",\\\"maxReceiveCount\\\":\\\"3\\\"}\"}"

### Get the 'cads-bridge-queue.fifo' queue ARN
queue_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$queue_url" \
  --attribute-name QueueArn \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-bridge-queue.fifo' queue ARN: $queue_arn"

## 'cads-mi-reports-queue.fifo'

### Create 'cads-mi-reports-queue.fifo' DLQ
dlq_url=$(awslocal sqs create-queue \
  --queue-name cads-mi-reports-queue-deadletter.fifo \
  --attributes '{"FifoQueue":"true","ContentBasedDeduplication":"true"}' \
  --output text \
  --query 'QueueUrl')
echo "'cads-mi-reports-queue.fifo' DLQ created: $dlq_url"

### Get the ARN of the 'cads-mi-reports-queue.fifo' DLQ, which is needed for the redrive policy
dlq_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$dlq_url" \
  --attribute-names QueueArn \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-mi-reports-queue.fifo' DLQ ARN: $dlq_arn"

### Create 'cads-mi-reports-queue.fifo' queue
queue_url=$(awslocal sqs create-queue \
  --queue-name cads-mi-reports-queue.fifo \
  --attributes '{"FifoQueue":"true","ContentBasedDeduplication":"true"}' \
  --output text \
  --query 'QueueUrl')
echo "'cads-mi-reports-queue.fifo' queue created: $queue_url"

### Define the Redrive Policy, linking the main queue to the DLQ.
redrive_policy_json=$(cat <<EOF
{
  "deadLetterTargetArn": "$dlq_arn",
  "maxReceiveCount": "3"
}
EOF
)

### Set redrive policy for 'cads-mi-reports-queue.fifo' DLQ
echo "Set redrive policy for 'cads-mi-reports-queue.fifo' DLQ..."
awslocal sqs set-queue-attributes \
  --queue-url "$queue_url" \
  --attributes "{\"RedrivePolicy\":\"{\\\"deadLetterTargetArn\\\":\\\"$dlq_arn\\\",\\\"maxReceiveCount\\\":\\\"3\\\"}\"}"

### Get the 'cads-mi-reports-queue.fifo' queue ARN
queue_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$queue_url" \
  --attribute-name QueueArn \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-mi-reports-queue.fifo' queue ARN: $queue_arn"

echo "LocalStack Bootstrapping Complete"