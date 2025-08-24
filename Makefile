COMPOSE_FILE=docker/docker-compose.yml

.DEFAULT_GOAL := help

up:
	docker compose -f $(COMPOSE_FILE) up -d --build

down:
	docker compose -f $(COMPOSE_FILE) down

logs:
	docker compose -f $(COMPOSE_FILE) logs -f

shell-frontend:
	docker compose -f $(COMPOSE_FILE) exec angular-frontend sh

shell-java-backend:
	docker compose -f $(COMPOSE_FILE) exec java-backend sh

shell-dotnet-backend:
	docker compose -f $(COMPOSE_FILE) exec dotnet-backend sh

restart-dotnet:
	docker compose -f $(COMPOSE_FILE) up -d --build dotnet-backend

shell-db:
	docker compose -f $(COMPOSE_FILE) exec postgres-db sh

psql:
	docker compose -f $(COMPOSE_FILE) exec postgres-db psql -U quiz-db -d quiz-db

help:
	@echo ""
	@echo "Available make commands:"
	@echo "  make up               – Starts all services in detached mode"
	@echo "  make down             – Stops and removes all containers"
	@echo "  make logs             – Shows container logs (follows output)"
	@echo "  make shell-frontend   – Opens a shell in the Angular frontend container"
	@echo "  make shell-backend-dotnet – Opens a shell in the .NET backend container"
	@echo "  make shell-db         – Opens a shell in the Postgres container"
	@echo "  make psql             – Opens psql CLI inside the Postgres container"
