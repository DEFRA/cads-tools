# CADS Development Tools: Harness

## Table of contents

- [Overview](#overview)

## Overview

This `harness` directory contains instructions and configuration for spinning up all local service dependencies required for the CADS solution.

## Starting infra + oidc

```
./harness/run-harness.sh up
```

## Stopping infra + oidc

```
./harness/run-harness.sh down
```

## Get a token from the oidc

Signs in through the browser using the authorization code flow and prints the tokens.

```
./harness/get-token.ps1                # cads-mis (default)
./harness/get-token.ps1 -App mis       # cads-mis
./harness/get-token.ps1 -App admin     # cads-admin-frontend
```

| App     | Client                      | Test user         | Default scopes                                                                         |
|---------|-----------------------------|-------------------|----------------------------------------------------------------------------------------|
| `mis`   | `local-cads-mis`            | `mip-viewer-user` | `reports.read`                                                                         |
| `admin` | `local-cads-admin-frontend` | `cads-admin-user` | `admin.db.execute`, `admin.s3.manager`, `admin.queue.manager`                          |

Both use the password `password`. Use `-Scopes "openid profile email ..."` to override the scopes.
Client details must match `oidc/config/clients.yml`.

## Get a token from the sts-mock

Requests a token from the STS mock (client credentials flow, no browser) and prints the decoded header and payload. See `sts-mock/ReadMe.md` for the clients and scopes.

```
./harness/get-sts-token.ps1 -App cads-bridge -Audience cads-data-service
./harness/get-sts-token.ps1 -App cads-data-service -Audience cads-bridge
./harness/get-sts-token.ps1 -App cads-bridge -Scopes "sts.cads-data-service"   # set the scope manually
```

| Parameter   | Meaning                                                                         |
|-------------|---------------------------------------------------------------------------------|
| `-App`      | Required. Who is calling (`cads-bridge` or `cads-data-service`).                |
| `-Audience` | Required. Who the token is for. The scope is derived as `sts.<audience>`.       |
| `-Scopes`   | Optional. Overrides the derived scope.                                          |

The secret is `local-mock-secret`. Client details must match `sts-mock/config/clients.yml`.
