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

## Get token for the oidc to be used for CADS CDS MiBff endpoints

```
./harness/get-token.ps1
```

Enter test credentials of `mip-viewer-user` and `password`.