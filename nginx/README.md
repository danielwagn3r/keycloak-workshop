# Usage

Before first run, edit [init-letsencrypt.sh](init-letsencrypt.sh) and set the `domains` and `email` as needed.

Initialize with

```bash
./init-letsencrypt.sh
```

To start the reverse proxy run:

```
docker compose up
```
