# Docker Setup Guide

This guide explains how to set up and run FlowingLinks using Docker and Docker Compose.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop) installed and running
- [Docker Compose](https://docs.docker.com/compose/install/) installed (included with Docker Desktop)
- Git (for cloning the repository)

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/FlowingLinks.git
cd FlowingLinks
```

### 2. Create Environment Configuration

Copy the example environment file and configure it for your setup:

```bash
cp .env.example .env
```

Then edit the `.env` file with your preferred settings (see [Configuration](#configuration) section below).

### 3. Start the Containers

```bash
docker-compose up -d
```

The `-d` flag runs containers in the background (detached mode).

### 4. Verify the Setup

Check if containers are running:

```bash
docker-compose ps
```

Access the application:
- **Frontend**: http://localhost:81 (or your configured `FRONTEND_PORT`)
- **Backend**: http://localhost:5000/swagger (or your configured `BACKEND_PORT`)

## Configuration

The `.env` file controls all configuration. Here are the available options:

### Database Configuration

```env
# Database Provider: SQLite or PostgreSQL
DB_PROVIDER=PostgreSQL

# PostgreSQL Settings (required if DB_PROVIDER=PostgreSQL)
DB_HOST=your-postgres-host.example.com
DB_PORT=5432
DB_NAME=FlowingLinks
DB_USER=postgres_user
DB_PASSWORD=your_very_secure_password_here

# SQLite Path (used if DB_PROVIDER=SQLite)
DB_PATH=/app/data/FlowingLinks.db
```

**Options:**
- **SQLite** (recommended for development): Simple file-based database, no external server needed
  ```env
  DB_PROVIDER=SQLite
  DB_PATH=/app/data/FlowingLinks.db
  ```

- **PostgreSQL** (recommended for production): Robust relational database
  ```env
  DB_PROVIDER=PostgreSQL
  DB_HOST=postgres.example.com
  DB_PORT=5432
  DB_NAME=FlowingLinks
  DB_USER=your_postgres_user
  DB_PASSWORD=your_secure_password
  ```

### Frontend Configuration

```env
# API URL that the frontend will use to communicate with the backend
VITE_API_URL=http://localhost:5000
```

Set this to:
- `http://localhost:5000` - For local development
- `http://your-domain.com` - For production (backend URL accessible from client)
- `http://backend:5000` - When frontend and backend are on the same Docker network (internal)

### JWT Configuration

```env
# JWT Secret Key (must be at least 32 characters for production)
JWT_KEY=your-secret-key-change-this-in-production-to-at-least-32-chars

# JWT Token Issuer
JWT_ISSUER=FlowingLinks

# JWT Token Audience
JWT_AUDIENCE=FlowingLinks

# JWT Token Expiration Time in minutes
JWT_EXPIRY_MINUTES=60
```

**Important Security Notes:**
- Change `JWT_KEY` to a strong, random value in production (minimum 32 characters)
- Example secure key: `openssl rand -hex 32` (on Linux/Mac) or use a password generator
- `JWT_EXPIRY_MINUTES` controls how long authentication tokens remain valid
- Default is 60 minutes; adjust based on your security requirements

### Port Configuration

```env
# Backend port (default: 5000)
BACKEND_PORT=5000

# Frontend port (default: 81 for production, 5173 for development)
FRONTEND_PORT=81
```

### Runtime Environment

```env
# ASP.NET Core environment: Production or Development
ASPNETCORE_ENVIRONMENT=Production
```

- `Production` - Optimized for deployment, minimal logging
- `Development` - Enhanced logging and debugging features

### Development Mode (Optional)

Uncomment these lines in `.env` for local development with hot reloading:

```env
ASPNETCORE_ENVIRONMENT=Development
BACKEND_VOLUMES=./Backend:/app
FRONTEND_VOLUMES=./Frontend:/app
STDIN_OPEN=true
TTY=true
FRONTEND_PORT=5173
DB_PROVIDER=SQLite
```

This configuration:
- Mounts local directories into containers for live code changes
- Enables interactive terminal input
- Uses development ports (5173 for frontend Vite server)
- Uses SQLite for easy local database management

## Common Operations

### Start Containers

```bash
docker-compose up -d
```

### Stop Containers

```bash
docker-compose down
```

### View Container Logs

```bash
# All containers
docker-compose logs -f

# Specific container
docker-compose logs -f backend
docker-compose logs -f frontend
```

### Rebuild Containers

If you modify Dockerfiles or dependencies:

```bash
docker-compose up -d --build
```

### Remove Everything (Reset)

```bash
docker-compose down -v
```

The `-v` flag removes associated volumes (databases, cached data).

### Access Container Shell

```bash
# Backend
docker exec -it flowinglinks-backend bash

# Frontend
docker exec -it flowinglinks-frontend sh
```

## Environment Variable Reference

| Variable | Default | Description |
|----------|---------|-------------|
| `DB_PROVIDER` | SQLite | Database type: SQLite or PostgreSQL |
| `DB_HOST` | (empty) | PostgreSQL host |
| `DB_PORT` | 5432 | PostgreSQL port |
| `DB_NAME` | (empty) | PostgreSQL database name |
| `DB_USER` | (empty) | PostgreSQL username |
| `DB_PASSWORD` | (empty) | PostgreSQL password |
| `DB_PATH` | /app/data/FlowingLinks.db | SQLite database path |
| `VITE_API_URL` | http://backend:5000 | Frontend API endpoint |
| `JWT_KEY` | your-secret-key-change-this-in-production | JWT signing secret key (min 32 chars) |
| `JWT_ISSUER` | FlowingLinks | JWT token issuer |
| `JWT_AUDIENCE` | FlowingLinks | JWT token audience |
| `JWT_EXPIRY_MINUTES` | 60 | JWT token expiration time in minutes |
| `BACKEND_PORT` | 5000 | Backend service port |
| `FRONTEND_PORT` | 81 | Frontend service port |
| `ASPNETCORE_ENVIRONMENT` | Production | ASP.NET Core environment |
| `BACKEND_VOLUMES` | . | Backend mount point (development) |
| `FRONTEND_VOLUMES` | . | Frontend mount point (development) |
| `STDIN_OPEN` | false | Enable stdin for backend container |
| `TTY` | false | Enable TTY for backend container |

## Setup Scenarios

### Scenario 1: Local Development (SQLite + Hot Reload)

```env
# Database
DB_PROVIDER=SQLite
DB_PATH=/app/data/FlowingLinks.db

# JWT
JWT_KEY=dev-secret-key-not-for-production
JWT_ISSUER=FlowingLinks
JWT_AUDIENCE=FlowingLinks
JWT_EXPIRY_MINUTES=1440

# Frontend
VITE_API_URL=http://localhost:5000

# Ports
BACKEND_PORT=5000
FRONTEND_PORT=5173

# Environment
ASPNETCORE_ENVIRONMENT=Development

# Development Mode
BACKEND_VOLUMES=./Backend:/app
FRONTEND_VOLUMES=./Frontend:/app
STDIN_OPEN=true
TTY=true
```

### Scenario 2: Production (PostgreSQL)

```env
# Database
DB_PROVIDER=PostgreSQL
DB_HOST=your-postgres-server.com
DB_PORT=5432
DB_NAME=FlowingLinks
DB_USER=prod_user
DB_PASSWORD=your_secure_password_123

# JWT (IMPORTANT: Use a strong random key in production!)
JWT_KEY=your-strong-random-key-minimum-32-characters-long-change-this
JWT_ISSUER=FlowingLinks
JWT_AUDIENCE=FlowingLinks
JWT_EXPIRY_MINUTES=60

# Frontend
VITE_API_URL=https://your-domain.com

# Ports
BACKEND_PORT=5000
FRONTEND_PORT=80

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

**JWT Key Generation Tips:**
- Linux/Mac: `openssl rand -hex 32`
- Windows PowerShell: `[Convert]::ToBase64String((1..32 | % {[byte](Get-Random -Min 0 -Max 256)}))`
- Online: Use a strong password generator (ensure at least 32 characters)

### Scenario 3: Staging (Docker Internal Network)

```env
# Database
DB_PROVIDER=PostgreSQL
DB_HOST=postgres-container
DB_PORT=5432
DB_NAME=FlowingLinks
DB_USER=stage_user
DB_PASSWORD=staging_password

# JWT
JWT_KEY=staging-secret-key-change-for-production
JWT_ISSUER=FlowingLinks
JWT_AUDIENCE=FlowingLinks
JWT_EXPIRY_MINUTES=120

# Frontend (internal Docker network)
VITE_API_URL=http://backend:5000

# Ports
BACKEND_PORT=5000
FRONTEND_PORT=8080

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

## Troubleshooting

### Containers won't start

1. Check logs:
   ```bash
   docker-compose logs
   ```

2. Verify `.env` file exists:
   ```bash
   ls -la .env
   ```

3. Ensure ports aren't already in use:
   ```bash
   # Linux/Mac
   lsof -i :5000
   lsof -i :81

   # Windows
   netstat -ano | findstr :5000
   netstat -ano | findstr :81
   ```

### Database connection errors

- If using PostgreSQL, verify host, port, credentials in `.env`
- Ensure PostgreSQL server is running and accessible
- Check firewall rules if PostgreSQL is on a remote server

### Frontend can't connect to backend

- Verify `VITE_API_URL` in `.env` matches the backend's actual address
- Check backend logs: `docker-compose logs backend`
- Verify backend health: http://localhost:5000/swagger

### Port conflicts

Change ports in `.env`:
```env
BACKEND_PORT=5001
FRONTEND_PORT=82
```

Then restart:
```bash
docker-compose up -d
```

### Data persistence issues

- SQLite database is stored in `./Backend/data/FlowingLinks.db`
- Ensure the `./Backend/data` directory exists and is writable
- Don't use `docker-compose down -v` unless you want to delete data

## Next Steps

- Review the [Backend README](./Backend/README.md) for ASP.NET Core details
- Review the [Frontend README](./Frontend/README.md) for Vue.js/Vite details
- Check [Contributing Guidelines](./CONTRIBUTING.md) for development workflow
