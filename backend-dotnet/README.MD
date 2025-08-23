# Backend Setup (ASP.NET Core + Docker)

## Lokales Setup ohne Docker Container

```bash
cd backend-dotnet/AspNetBackend
dotnet run
```
Alternative: In der IDE (z. B. Visual Studio oder Rider) das File Program.cs direkt starten.

Danach kann man unter [http://localhost/swagger/index.html](http://localhost/swagger/index.html) die Swagger API nutzen.

**Wichtig:** Der Postgres-Datenbank-Container muss dabei laufen, 
sonst schlagen die Webrequests fehl. Dies muss im Projekt-Root gestartet werden.

```bash
docker compose -f docker/docker-compose.yml up -d postgres-db
```

---

## Docker Build

```bash
cd backend-dotnet/AspNetBackend
```

```bash
docker build -t backend-dotnet .
```

Build ohne Cache:

```bash
docker build --no-cache -t backend-dotnet .
```

---

## Docker Cleanup

Alle Images löschen:

```bash
docker rmi -f $(docker images -q)
```

---

## Container bauen und starten via Docker Compose (im Projekt-Root)

Dabei wird der Postgres-Datenbank-Container automatisch mitgestartet.

```bash
docker compose -f docker/docker-compose.yml up -d --build dotnet-backend
```
Nur den Postgres-Datenbank-Container separat starten

```bash
docker compose -f docker/docker-compose.yml up -d postgres-db
```

So kann beispielsweise das Backend lokal laufen, während die Datenbank im Container verfügbar ist.



---

# PostgreSQL Datenbank

Alle Einträge der Tabelle Players anzeigen:

```sql
SELECT * FROM players."players";
```
Alle Einträge der Tabelle Leaderboard anzeigen:

```sql
SELECT * FROM players."leaderboard";
```

---

# Entity Framework Core Migrationen

Migration prüfen (zeigt SQL-Script):

```bash
dotnet ef migrations script
```

## Voraussetzungen für Migrationen

EF-Tools installieren:

```bash
dotnet tool install --global dotnet-ef
```

Migration erstellen:

```bash
dotnet ef migrations add InitialCreatePlayerAndLeaderBoard
```

Datenbank aktualisieren:

```bash
dotnet ef database update
```

---

## Migrationen verwalten

Alle Migrationen anzeigen:

```bash
dotnet ef migrations list
```

Zurück auf bestimmten Stand:

```bash
dotnet ef database update InitialCreatePlayerAndLeaderBoard
```

Letzte Migration entfernen:

```bash
dotnet ef migrations remove
```

Optional Migration mit explizitem DbContext:

```bash
dotnet ef migrations add InitialCreate --context AspNetPostgresDbContext
```
