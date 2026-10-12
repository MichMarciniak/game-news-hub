
# --- Docker db ---

run-db:
	docker compose --file docker-compose.yml up -d
db:
	docker exec -it postgres_db psql -U myuser -d mydb


# --- Migracje DB --- 

PROJECT ?= GameNewsHub.Data
STARTUP ?= GameNewsHub.Api

.PHONY: mig update drop reset

mig:
	dotnet ef migrations add $(NAME) --project $(PROJECT) --startup-project $(STARTUP)
    
update:
	dotnet ef database update --project $(PROJECT) --startup-project $(STARTUP)
    
drop:
	dotnet ef database drop --project $(PROJECT) --startup-project $(STARTUP)
    
reset: drop update
