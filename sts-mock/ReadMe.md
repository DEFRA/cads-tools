# CADS Development Tools: STS mock

## Table of contents

- [Overview](#overview)

## Overview

This `sts-mock` directory contains the configuration for a local stand-in for AWS STS outbound identity federation (`GetWebIdentityToken`). LocalStack does not support it. It is started by the harness together with infra and oidc, and serves OIDC discovery plus signing keys on `http://cads-sts-mock` (port `5567` on the host). Services running in Docker on `cads-network` use `http://cads-sts-mock`.

## Starting and stopping

Started and stopped by the harness:

```
./harness/run-harness.sh up
./harness/run-harness.sh down
```

## Check it is running

```
curl -s http://localhost:5567/.well-known/openid-configuration
```

## Get a token from the sts-mock

Plain REST, no AWS signing. The token has `iss`, `aud`, `sub` (role ARN) and the nested `https://sts.amazonaws.com/` claim, like a real AWS token.

```
curl -s -X POST http://localhost:5567/connect/token -d grant_type=client_credentials -d client_id=local-sts-cads-bridge -d client_secret=local-mock-secret -d scope=sts.cads-cds
```

Use `scope` to choose the audience (the service the token is for):

| Scope             | Token `aud` (audience)    |
| ----------------- | ------------------------- |
| `sts.cads-cds`    | `sts://local-cads-cds`    |
| `sts.cads-bridge` | `sts://local-cads-bridge` |

Use `client_id` to choose who is calling. All clients use the secret `local-mock-secret`.

| Client                        | Caller (`ServiceName`) | Purpose      |
| ----------------------------- | ---------------------- | ------------ |
| `local-sts-cads-bridge`       | `cads-bridge`          | valid caller |
| `local-sts-cads-data-service` | `cads-data-service`    | valid caller |

Local values: `org_id` = `o-localmock`, `principal_tags.Environment` = `local`.

Notes:

- The signing key will change when the containers are recreated (`down` + `up`). While a plain restart, will retain it.
- This is HTTP only, so the receiving service needs a local-dev setting that allows HTTP discovery.
- Client details must match `sts-mock/config/clients.yml`.
