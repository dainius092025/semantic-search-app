# 📚 Semantic Search for Short Story Publisher

A fullstack application that allows users to search for short stories 
using natural language.

## Architecture

| Layer | Technology |
|---|---|
| Frontend | React |
| Backend API | C# / ASP.NET Core |
| Database | PostgreSQL + pgvector |
| LLM / Embeddings | Ollama (nomic-embed-text) |
| Infrastructure | Docker / Docker Compose |

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)
- [VSCode](https://code.visualstudio.com/)
- [.NET SDK 10](https://dotnet.microsoft.com/en-us/download)

## Getting Started

### 1. Clone the repository
git clone <your-repo-url>
cd <your-repo-folder>
git checkout dev

### 2. Start Docker containers
docker compose up -d

### 3. Pull the Ollama model
docker exec -it semantic_ollama ollama pull nomic-embed-text

### 4. Enable pgvector
docker exec -it semantic_db psql -U admin -d semanticdb 
-c "CREATE EXTENSION IF NOT EXISTS vector;"

## Team

| Role | Responsibility |
|---|---|
| Product Owner / Backend Dev | Issues, documentation, backlog, backend |
| Backend Dev 1 | Database schema |
| Backend Dev 2 | Story ingestion + seeding |
| Backend Dev 3 | Ollama service |
| Frontend Dev 1 | Frontend development |
| Frontend Dev 2 | Frontend development |
| Fullstack Dev | Frontend + backend support |

## In Progress
- Database schema
- Ollama service integration
- API endpoints (coming soon)