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
