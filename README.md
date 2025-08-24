# OOP-Project Online-Quiz

## Team 
Alexander Süess-Winter, Ramon Schöni, Jonas Tscharner

## Beschreibung
Ein Online Quiz, an welchem über ein Link/QR-Code über eine Webapplikation teilgenommen werden kann.

## MVP (Minimum Viable Product)
 
 - Stufe 1: "Statisches" Quiz (in DB oder File), Score mit Benutzername in DB speichern und vergleichen mit anderen Scores.

### Erweiterungsmöglichkeiten

 - Erweiterung 1: Dynamisches Quiz mit KI generierten Quizfragen durch API-Abfragen (Möglicherweise mehrere APIs).
- Erweiterung 2: Spielerprofil (User Login) und Benutzeroberfläche um selber Quiz zu erstellen.
 - Erweiterung 3: Live Quiz gegen andere (mit Live-Scores).

## Technologien

| Komponente         | Technologie                           |
|--------------------|----------------------------------------|
| Frontend (Web)     | JavaScript/React, Blazor (C#) oder Angular |
| Primärer Backend   | Java Spring Boot                      |
| Sekundärer Backend | C# ASP.NET Core                       |
| Kommunikation      | REST                                  |
| Hosting            | Docker, Docker-Compose                |

- Backend 1: SpringBoot für Webserver und Quizmanagement
- Backend 2: ASP.NET für zusätzliche Features (Microservices)
- Frontend: React.js, Angular oder ein anderes Framework? (Je nach dem was wir im Modul benutzen)

# Installation Instruction

## Wie builde und starte ich die Container?

Im Root-Verzeichnis des Projekts befindet sich ein Makefile. Das Makefile enthält vordefinierte Kommandos, die den Umgang mit `docker compose` vereinfachen.

Beispiel:

```bash
make up
```

Entspricht:

```bash
docker compose -f docker/docker-compose.yml up -d --build
```

## Übersicht der `make` Befehle

| Befehl                      | Beschreibung                                  |
| --------------------------- | --------------------------------------------- |
| `make up`                   | Startet alle Container im Hintergrund         |
| `make down`                 | Stoppt und entfernt alle Container            |
| `make logs`                 | Zeigt Logs aller Container, folgt der Ausgabe |
| `make shell-frontend`       | Öffnet eine Shell im Angular-Frontend         |
| `make shell-dotnet-backend` | Öffnet eine Shell im .NET Backend             |
| `make shell-java-backend`   | Öffnet eine Shell im Java Backend             |
| `make shell-db`             | Öffnet eine Shell in der Postgres-Datenbank   |
| `make psql`                 | Öffnet das psql CLI in der Datenbank          |
| `make restart-dotnet`       | Baut und startet nur den .NET Backend neu     |

---

# Installation von `make` auf Windows & macOS

## macOS

`make` ist oft bereits vorinstalliert.

### Prüfen:

```bash
make --version
```

Falls nicht vorhanden, Installation über die Xcode Command Line Tools:

```bash
xcode-select --install
```

Die Installation enthält nur die notwendigen Entwickler-Werkzeuge, kein komplettes Xcode.

---

## Windows

`make` muss unter Windows manuell nachgerüstet werden.

### 1. Installation über Chocolatey (empfohlen)

```powershell
choco install make
```

Danach prüfen:

```powershell
make --version
```

Chocolatey installieren: [https://chocolatey.org/install](https://chocolatey.org/install)

---

# Alternativen zu `make`

Falls kein `make` installiert ist, lassen sich alle Aktionen manuell mit `docker compose` ausführen.

### Alle Services starten

```bash
docker compose -f docker/docker-compose.yml up -d --build
```

### Alle stoppen

```bash
docker compose -f docker/docker-compose.yml down
```

### Logs verfolgen

```bash
docker compose -f docker/docker-compose.yml logs -f
```

---

## Container Shell öffnen

### Frontend (Angular)

```bash
docker compose -f docker/docker-compose.yml exec angular-frontend sh
```

### Backend (.NET)

```bash
docker compose -f docker/docker-compose.yml exec dotnet-backend sh
```

### Backend (Java)

```bash
docker compose -f docker/docker-compose.yml exec java-backend sh
```

### Postgres

```bash
docker compose -f docker/docker-compose.yml exec postgres-db sh
```

---

## Datenbank CLI (psql) öffnen

```bash
docker compose -f docker/docker-compose.yml exec postgres-db psql -U quiz-db -d quiz-db
```

---

## Nur das .NET Backend neu bauen und starten

```bash
docker compose -f docker/docker-compose.yml up -d --build dotnet-backend
```

## Nur das Java Backend neu bauen und starten

```bash
docker compose -f docker/docker-compose.yml up -d --build java-backend
```

## Nur das Angular Frontend neu bauen und starten

```bash
docker compose -f docker/docker-compose.yml up -d --build angular-frontend
```
---

# Nutzung der Software

Sobald alle Container laufen, ist das Angular-Frontend erreichbar unter:

```
http://localhost:4200
```

Nach erfolgreichem Registrieren kann über den Button **Quiz** das Quiz begonnen werden.

Die Player und Leaderboard Tabelle können nur nach erfolgreichen Login angezeit werden.









