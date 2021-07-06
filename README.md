# ShipWithMe-server

### Setup
- Download [.NET 5](https://dotnet.microsoft.com/download).
- Download [Docker](https://docs.docker.com/get-docker/).

- Run the following command to install dotnet-ef which is required to run migrations.
```bash
./do --install-tools
```

- Create local self signed ssl certificates and add shipnshare.local to your host file (ex. /private/etc/hosts)
```bash
./do --gen-cert
```

- Create the ShipWithMeWeb/secrets.develop.json file which holds additional info required for the server and fill in the fields not automatically generated.
```bash
./do --cdsecrets
```

- Run all database migrations
```bash
./do --db start
./do --db init
```

### How to start the server
- Start the database server (if not already started)

```bash
./do --db start
```

- Start the server
```bash
./do --api
```

### Additional commands
```bash
./do -h
```
