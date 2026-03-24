# Semantic Search App

A full-stack semantic search application built for a short story publisher.
Users can search for short stories using natural language — the system 
finds stories based on meaning and theme, not just exact words.

Built as a team learning project by a team of 6 developers 
(4 backend, 1 fullstack, 1 frontend) to explore semantic search, 
vector embeddings, and AI-powered text processing.

## Tech Stack
- **Frontend:** React, Vite
- **Backend:** ASP.NET Core (.NET 10)
- **Database:** PostgreSQL with pgvector
- **AI:** Ollama (nomic-embed-text for embeddings, gemma3:1b for summaries)
- **Infrastructure:** Docker, Docker Compose

## Prerequisites

Make sure you have the following installed before running the project:

- [Docker](https://www.docker.com/get-started) (version 20 or higher)
- [Docker Compose](https://docs.docker.com/compose/install/) (included with Docker Desktop)

That is all you need — everything else (database, backend, frontend, 
and AI models) runs inside Docker automatically.    

## How to Run

### 1. Clone the repository
```bash
git clone https://github.com/dainius092025/semantic-search-app.git
cd semantic-search-app
```

### 2. Start the application
```bash
docker compose up --build
```

This will automatically:
- Start the PostgreSQL database
- Start Ollama and download the required AI models
- Build and start the backend
- Build and start the frontend

### 3. Wait for Ollama models to download
The first run will take a few minutes while Docker downloads
the AI models. You will see download progress in the terminal.

### 4. Load stories into the database
Once all services are running, open a new terminal and run:
```bash
curl -X POST http://localhost:5162/api/ingestion/run
```

### 5. Open the application
- **Frontend:** http://localhost:80
- **Backend API:** http://localhost:5162

### Stopping the application
```bash
docker compose down
```

## API Endpoints

### Search
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/search` | Hybrid search using semantic and keyword matching |
| POST | `/api/search/metadata` | Keyword only search by title, author, genre or year |

### Stories
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/stories` | Returns all stories in the database |
| GET | `/api/stories/{id}` | Returns a single story by ID |

### Ingestion
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/ingestion/run` | Loads and processes all stories into the database |

### Request Example
```json
POST /api/search
{
    "query": "lonely person at sea",
    "limit": 5
}
```

### Response Example
```json
[
    {
        "id": 1,
        "title": "The Old Man and the Sea",
        "author": "Ernest Hemingway",
        "year": 1952,
        "genre": "Fiction",
        "summary": "An old fisherman struggles alone at sea.",
        "similarity": 0.86
    }
]
```

## Architecture Overview

The application consists of four main services that run together in Docker:
```
User Browser
     ↓
Frontend (React) — http://localhost:80
     ↓
Backend (ASP.NET Core) — http://localhost:5162
     ↓                    ↓
PostgreSQL            Ollama
(pgvector)            (AI models)
```

### How Search Works
1. User enters a search query in the frontend
2. Frontend sends the query to the backend API
3. Backend generates an embedding vector using Ollama
4. Backend runs semantic search against PostgreSQL using pgvector
5. Backend runs keyword search against story metadata
6. Results are combined using weighted ranking (70% semantic, 30% keyword)
7. Top results are returned to the frontend

### Documentation
- [Search Architecture](docs/search-architecture.md) — detailed explanation of how search works
- [Design Thinking](docs/design-thinking-ideation.md) — original design ideas
- [Testing and Iteration](docs/design-thinking-testing.md) — user feedback and decisions
- [Reflection](docs/reflection.md) — what went well, challenges and future improvements

## Contributors

| Name | Role |
|---|---|
| [name] | Backend Developer |
| [name] | Backend Developer |
| [name] | Backend Developer |
| [name] | Backend Developer |
| [name] | Fullstack Developer |
| [name] | Frontend Developer |

## Links
- [Search Architecture Documentation](docs/search-architecture.md)
- [Design Thinking — Ideation](docs/design-thinking-ideation.md)
- [Design Thinking — Testing and Iteration](docs/design-thinking-testing.md)
- [Project Reflection](docs/reflection.md)
- [GitHub Issues](https://github.com/dainius092025/semantic-search-app/issues)

## License
This project was built for learning purposes.
