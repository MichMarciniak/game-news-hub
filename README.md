# Game News Hub

Game News Hub is a project focused on aggregating and personalizing video game event/news.

# How To Run

## Prerequisites

- .NET 10 SDK
- Docker
- node

## Containers
Run in `backend` dir
```sh
docker compose -f docker-compose.yml up -d
```

## Backend
### 1) Configure secrets/env vars

In Sync project:
- `Api:ClientId`
- `Api:ClientSecret`

### 2) Migrate db

Run `Migrator` project or update db manually (should work)
```sh
dotnet ef database update --project GameNewsHub.Data --startup-project GameNewsHub.Api
```

### 3) Start api

in `Api` project run
```sh
dotnet run
```

### 4) Endpoints

- Scalar: `http://localhost:5190/scalar`

## Frontend
In `frontend-vue` run `npm i` and `npm run dev`

### Generate api from backend docs
Backend must be running/docs must be available
`orval.config.ts` - `input: {target: url}`
```sh
npm run generate:api
```
It will create api hooks in `src/api/generated`
