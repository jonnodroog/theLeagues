# TheLeagues

A football league standings platform built with a microservices architecture. TheLeagues pulls live data from a third-party API, stores it in a PostgreSQL database, and serves it through a Blazor WebAssembly frontend.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?logo=blazor)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)

---

## What It Does

Users can browse football leagues (Premier League, Ligue 1, Serie A, La Liga, Bundesliga, and more), select a league, and view the current standings table — including each team's rank, points, goal difference, wins, draws, and losses.
Data is kept up to date by triggering sync jobs against the external API, which run asynchronously via a background task queue so the API stays responsive.

---

## Architecture

The project is split into four services, all orchestrated with Docker Compose:

```
┌─────────────────────────────────────────────────────────┐
│                    Docker Compose                        │
│                                                         │
│  ┌──────────────┐    ┌──────────────┐                   │
│  │   AwayAPI    │    │   HomeAPI    │                   │
│  │  (port 5002) │    │  (port 5001) │                   │
│  │              │    │              │                   │
│  │ Data ingestion    │ Data serving │                   │
│  │ from external│    │ to the UI    │                   │
│  │ football API │    │              │                   │
│  └──────┬───────┘    └──────┬───────┘                   │
│         │                  │                            │
│         └────────┬─────────┘                            │
│                  │                                      │
│         ┌────────▼────────┐                             │
│         │   PostgreSQL 16  │                             │
│         │   (port 5432)    │                             │
│         └─────────────────┘                             │
│                                                         │
│  ┌──────────────────────┐                               │
│  │   TheLeaguesUI        │                               │
│  │   Blazor WASM (80)    │                               │
│  │   Served via Nginx    │                               │
│  └──────────────────────┘                               │
└─────────────────────────────────────────────────────────┘
```

### Services

| Service | Purpose |
|---|---|---|
| **AwayAPI** | Fetches data from the third-party API and writes to the database via background jobs
| **HomeAPI** | Reads from the database and serves data to the UI
| **TheLeaguesUI** | Blazor WebAssembly SPA, served by Nginx
| **PostgreSQL** | Shared database 

---

## Tech Stack

- **Backend**: ASP.NET Core Minimal APIs (.NET 10)
- **Frontend**: Blazor WebAssembly (.NET 10)
- **Database**: PostgreSQL 16 with Entity Framework Core
- **Containerisation**: Docker & Docker Compose

---

## Data Model

Three core entities are stored and served:

**League** — Name, country, logo, and a collection of teams.

**Team** — Name, code, country, founding date, logo, current league rank, points, goal difference, and full match stats (played, won, drawn, lost, goals for/against). Belongs to a league and has a collection of players.

**Player** — Name, age, nationality, height, squad number, position, and photo. Belongs to a team.

---

## API Endpoints

### AwayAPI — Data Ingestion

Triggers background sync jobs that fetch from api-sports.io and update the database.

| Method | Route | Description |
|---|---|---|
| `POST` | `/leagues` | Sync all configured leagues |
| `POST` | `/leagues/{id}` | Sync a specific league |
| `POST` | `/teams` | Sync all teams |
| `POST` | `/teams/{id}` | Sync a specific team |
| `POST` | `/players` | Sync all players |
| `POST` | `/players/{id}` | Sync a specific player |
| `GET` | `/health` | Health check |

### HomeAPI — Data Serving

Read-only endpoints consumed by the UI.

| Method | Route | Description |
|---|---|---|
| `GET` | `/leagues` | All leagues |
| `GET` | `/leagues/{id}` | League by ID |
| `GET` | `/team` | All teams |
| `GET` | `/team/teams/{id}` | Team by ID |
| `GET` | `/team/league-id={id}` | Teams by league |
| `GET` | `/players` | All players |
| `GET` | `/players/{id}` | Player by ID |
| `GET` | `/health` | Health check |

---

## Background Task Queue

AwayAPI uses a hosted background service to process data sync jobs asynchronously. When a sync endpoint is hit, the work item is enqueued and the API returns immediately. The `QueuedHostedService` processes items from the queue one at a time, keeping the API non-blocking and the sync jobs isolated.

---

## Project Structure

```
theLeagues/
├── AwayAPI/                  # Data ingestion service
│   ├── BackgroundServices/   # Task queue (IBackgroundTaskQueue, QueuedHostedService)
│   ├── DAL/                  # AwayDBContext
│   ├── Endpoints/            # Minimal API route definitions
│   ├── Extensions/           # Service registration, settings
│   ├── Migrations/           # EF Core migrations
│   └── Services/             # LeagueService, TeamService, PlayerService
├── HomeAPI/                  # Data serving service
│   ├── DAL/                  # HomeDBContext
│   ├── Endpoints/            # Minimal API route definitions
│   ├── Extensions/           # Service registration, settings
│   └── Services/             # LeagueService, TeamService, PlayerService
├── TheLeaguesUI/             # Blazor WebAssembly frontend
│   ├── Pages/                # Home.razor
│   ├── Pages/Components/     # LeagueCard, LogPanel
│   ├── Layout/               # MainLayout, NavBar
│   └── Services/             # API client services
├── Models/                   # Shared DTOs and domain models
├── docker-compose.yml
└── README.md
```

---

## Configured Leagues

The following leagues are tracked by default (configurable via `LeagueIdNumbers` in appsettings):

| ID | League |
|---|---|
| 39 | Premier League (England) |
| 61 | Ligue 1 (France) |
| 78 | Bundesliga (Germany) |
| 88 | Eredivisie (Netherlands) |
| 135 | Serie A (Italy) |
| 140 | La Liga (Spain) |
