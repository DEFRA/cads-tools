#!/bin/bash
sed -i 's/\r$//' "$0"

export AWS_REGION=eu-west-2
export AWS_DEFAULT_REGION=eu-west-2
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export ENDPOINT_URL=http://localhost:4566

set -e

# S3
echo "Creating S3 resources..."

## S3: Create 'cads-internal-bucket' bucket
existing_bucket=$(awslocal s3api list-buckets \
  --query "Buckets[?Name=='cads-internal-bucket'].Name" \
  --output text)

if [[ "$existing_bucket" == "cads-internal-bucket" ]]; then
  echo "S3 bucket already exists: cads-internal-bucket"
else
  awslocal s3api create-bucket --bucket cads-internal-bucket --region eu-west-2 \
    --create-bucket-configuration LocationConstraint=eu-west-2 \
    --endpoint-url=http://localhost:4566
  echo "S3 bucket created: cads-internal-bucket"
fi

# SQS
echo "Creating SQS resources..."

## S3: Create 'cads-cds-queue' DLQ
dlq_url=$(awslocal sqs create-queue \
  --queue-name cads-cds-queue-deadletter \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'QueueUrl')
echo "'cads-cds-queue' DLQ created: $dlq_url"

# Get the ARN of the 'cads-cds-queue' DLQ, which is needed for the redrive policy
dlq_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$dlq_url" \
  --attribute-names QueueArn \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-cds-queue' DLQ ARN: $dlq_arn"

# Create 'cads-cds-queue' queue
queue_url=$(awslocal sqs create-queue \
  --queue-name cads-cds-queue \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'QueueUrl')
echo "'cads-cds-queue' queue created: $queue_url"

# Define the Redrive Policy, linking the main queue to the DLQ.
redrive_policy_json=$(cat <<EOF
{
  "deadLetterTargetArn": "$dlq_arn",
  "maxReceiveCount": "3"
}
EOF
)

# Set redrive policy for 'cads-cds-queue' DLQ
echo "Set redrive policy for 'cads-cds-queue' DLQ..."
awslocal sqs set-queue-attributes \
  --queue-url "$queue_url" \
  --attributes "{\"RedrivePolicy\":\"{\\\"deadLetterTargetArn\\\":\\\"$dlq_arn\\\",\\\"maxReceiveCount\\\":\\\"3\\\"}\"}" \
  --endpoint-url=$ENDPOINT_URL
# =================================================================

# Get the 'cads-cds-queue' queue ARN
queue_arn=$(awslocal sqs get-queue-attributes \
  --queue-url "$queue_url" \
  --attribute-name QueueArn \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'Attributes.QueueArn')
echo "'cads-cds-queue' queue ARN: $queue_arn"

echo "LocalStack Bootstrapping Complete"