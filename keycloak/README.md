# Usage

Before first run, edit [Dockerfile](Dockerfile) and [docker-compose.yaml](docker-compose.yaml) and set the configuration parameters as needed. E.g. set the `KC_HOSTNAME_URL` appropriately.

Initialize with

```bash
docker compose build --no-cache
```

To start keycloak run:

```
docker compose up
```
