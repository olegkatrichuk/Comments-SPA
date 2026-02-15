# Comments SPA

A single-page application for comments with a cascading reply system, real-time updates, and full-text search. Implementation level: **Middle+** (architecture designed for 1M messages, 100K users/24h).

Architecture inspired by [Microsoft eShop](https://github.com/dotnet/eShop) — Clean Architecture, CQRS, Domain Events.

## Tech Stack

| Component | Technology |
|-----------|-----------|
| Backend | .NET 9, ASP.NET Core Web API |
| ORM | Entity Framework Core 9 |
| Frontend | Next.js 16 (App Router, TypeScript, Tailwind CSS) |
| Database | PostgreSQL 17 |
| Cache | Redis 7 |
| Message Broker | RabbitMQ 4 (via MassTransit) |
| Search | Elasticsearch 8 |
| Real-time | SignalR (Redis backplane) |
| GraphQL | Hot Chocolate 14 |
| CAPTCHA | Custom (SkiaSharp) |
| Containerization | Docker Compose |
| Load Tests | NBomber |

## Project Structure

```
├── src/
│   ├── Comments.Domain/              # Entities, Value Objects, Domain Events, interfaces
│   ├── Comments.Application/         # CQRS (commands/queries), DTOs, validators, MediatR
│   ├── Comments.Infrastructure/      # EF Core, Redis, RabbitMQ, Elasticsearch, SignalR
│   ├── Comments.API/                 # REST API, GraphQL, SignalR Hub, Middleware
│   └── Comments.WebApp/             # Next.js frontend
├── tests/
│   ├── Comments.UnitTests/
│   ├── Comments.IntegrationTests/
│   └── Comments.LoadTests/          # NBomber load test scenarios
├── docker/
│   └── docker-compose.yml
├── Dockerfile.api
└── Dockerfile.web
```

## Features

### Comments
- Create comments with fields: UserName, Email, HomePage (optional), Text
- Cascading (tree-based) reply system with unlimited nesting
- Pagination with 25 comments per page
- Sorting by UserName, Email, creation date (ASC/DESC)
- Default order: LIFO (newest first)

### Security
- CAPTCHA — custom image generation via SkiaSharp, answers stored in Redis with TTL
- XSS protection — HTML sanitization (Ganss.XSS), only allowed tags: `<a>`, `<code>`, `<i>`, `<strong>`
- XHTML tag closure validation
- Rate Limiting — 10 requests/min per IP for POST endpoints
- Client-side (Zod) and server-side (FluentValidation) validation

### Files
- Image upload (JPG, GIF, PNG) — automatic resizing to 320x240 via SkiaSharp
- Text file upload (TXT, max 100KB)
- Drag & drop upload
- Lightbox preview with animations

### HTML Toolbar
- Tag insertion buttons `[i]`, `[strong]`, `[code]`, `[a]` wrapping selected text
- AJAX comment preview without page reload

### Real-time
- SignalR WebSocket — new comments appear for all users without page reload
- Redis backplane for horizontal scaling

### Search
- Full-text search via Elasticsearch
- Search by UserName, Email, and comment text

### GraphQL
- Queries: get comment list, comment by ID, search
- Mutations: create comment
- Subscriptions: subscribe to new comments

## Getting Started

### Docker Compose (recommended)

```bash
cd docker
docker compose up --build
```

Services:
- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger (Development only)
- **GraphQL Playground**: http://localhost:5000/graphql
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Health Check**: http://localhost:5000/health

### Local Development

Requirements: .NET 9 SDK, Node.js 20+, PostgreSQL, Redis, RabbitMQ, Elasticsearch.

**Backend:**
```bash
dotnet restore
dotnet build
cd src/Comments.API
dotnet run
```

**Frontend:**
```bash
cd src/Comments.WebApp
npm install
npm run dev
```

## API Endpoints

### REST

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/comments` | List comments (pagination + sorting) |
| GET | `/api/comments/{id}` | Comment by ID with nested replies |
| POST | `/api/comments` | Create comment (multipart/form-data) |
| GET | `/api/captcha` | Generate CAPTCHA (image + key) |
| GET | `/api/files/{id}` | Get attached file |
| GET | `/api/search` | Full-text comment search |

### GraphQL

Endpoint: `/graphql`

```graphql
# Query comments
query {
  comments(page: 1, pageSize: 25, sortField: CREATED_AT, sortDirection: DESCENDING) {
    items { id userName email text createdAt replies { id text } }
    totalCount page pageSize hasNextPage
  }
}

# Subscribe to new comments
subscription {
  onCommentCreated { id userName text createdAt }
}
```

### SignalR Hub

Endpoint: `/hubs/comments`

Events:
- `ReceiveComment` — new comment created

## Database Schema

### Table `comments`

| Column | Type | Description |
|--------|------|-------------|
| id | UUID (v7) | Primary key (time-sortable) |
| user_name | VARCHAR(50) | Username (letters + digits) |
| email | VARCHAR(254) | Email |
| home_page | VARCHAR(2048) | Home page URL (nullable) |
| text | TEXT | Comment text (max 8000) |
| created_at | TIMESTAMPTZ | Creation date |
| parent_comment_id | UUID | FK to parent comment (nullable) |

Indexes: created_at DESC, user_name, email, parent_comment_id, partial index on top-level comments.

### Table `attachments`

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| file_name | VARCHAR(255) | Original file name |
| stored_file_name | VARCHAR(255) | Storage file name |
| content_type | VARCHAR(100) | MIME type |
| file_size_bytes | BIGINT | File size |
| type | INTEGER | 0 = Image, 1 = TextFile |
| comment_id | UUID | FK to comment (CASCADE) |

## Architectural Decisions for High Load

- **UUIDv7** — time-sortable primary keys, no sequence contention
- **Partial index** — filtered index on top-level comments (`WHERE parent_comment_id IS NULL`)
- **Redis cache-aside** — caching paginated queries (TTL 5 min), invalidation on write
- **Async processing** — Elasticsearch indexing and SignalR broadcast via RabbitMQ consumer (off the request path)
- **AsSplitQuery()** — preventing cartesian explosion when loading nested comments
- **Rate Limiting** — Fixed Window: 10 comments/min per IP
- **Redis backplane** — horizontal scaling for SignalR

## Load Testing

NBomber scenarios (3 minutes each):
- `read_comments` — 100 req/sec, paginated reads
- `create_comments` — 50 req/sec, comment creation
- `search_comments` — 80 req/sec, full-text search

```bash
cd tests/Comments.LoadTests
dotnet run -- http://localhost:5000
```

Report saved to `reports/load_test_report.html`.
