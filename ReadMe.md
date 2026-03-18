# CADS Development Tools

## Table of contents

- [Overview](#overview)
- [CADS Repository overview](#cads-repository-overview)

## Overview

The CADS Development Tools will provide the shared tools harness. 

These shared tools with then be used by frontend and backend repositories for local development, GitHub Actions pipelines and e2e journey tests.

The purpose for this repository is to solve the multi‑repo problem cleanly so that:

 - Frontend (UI) repo's don't need to know how to start infra
 - Backend repo's don't need to know how to start infra
 - e2e journey tests don't need to know how to start infra
 - The infra is defined once, in a dedicated repo
 - GitHub Actions can check out all three repos
 - Everything stays consistent
 - No duplication
 - No drift (when using duplicate OIDC mocks across multiple repo's)
 - No cross‑repo coupling

## CADS Repository overview

### cads-mis frontend

Contains UI code + UI compose.

- Has its own compose
- References tools repo in GitHub Actions
- Uses shared OIDC mock for local dev + e2e

### cads-cds backend

Contains only backend code + backend compose.

- Has its own compose
- References tools repo in GitHub Actions
- Uses shared OIDC mock for local dev + E2E
- Uses Testcontainers OIDC mock for integration tests

### cads-tools

Contains:
- OIDC mock
- LocalStack
- Postgres
- Redis
- Shared compose
- Shared env
- Shared scripts
- e2e harness

### cads-journey-tests

Contains e2e playwright code.

The e2e journey tests will run in three different contexts, and each one has different authentication requirements. The key is to use the OIDC mock where it makes sense, and the real Azure AD flow where it matters.

#### Local development (Frontend + Backend running locally)

 - Use the OIDC mock
 - Use the shared tools harness

#### e2e tests in GitHub Actions (CI)

 - Use the OIDC mock
 - Use the shared tools harness

#### e2e tests against a real physical environment (Azure/AWS)

 - Use the real Azure AD / Entra ID login flow
 - No mock, shortcuts or bypass
 - Use a dedicated Azure AD test user with no MFA enabled

### About the licence

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable
information providers in the public sector to license the use and re-use of their information under a common open
licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.