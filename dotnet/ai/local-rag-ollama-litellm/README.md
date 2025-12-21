# Local RAG with Ollama, LiteLLM, and Qdrant

A .NET implementation of a local Retrieval-Augmented Generation (RAG) system using Ollama for models, LiteLLM as an API proxy, and Qdrant for vector search.

## Overview

This project demonstrates how to build a fully local RAG pipeline that:

## Prerequisites

  - `nomic-embed-text` for embeddings
  - `qwen2.5` or `llama3.2` for generation

## Setup Guide

### Step 1: Install Prerequisites

#### Install Ollama

Download and install Ollama from [https://ollama.ai/](https://ollama.ai/)

Pull the required models:

```bash
ollama pull nomic-embed-text
ollama pull qwen2.5
```

Verify models are installed:
```bash
ollama list
```

#### Install LiteLLM

```bash
pip install litellm
```

#### Install Docker (for Qdrant)

Download and install Docker Desktop from [https://www.docker.com/](https://www.docker.com/)

### Step 2: Start Services

#### Start Ollama

Ollama should start automatically after installation. Verify it's running:
```bash
curl http://localhost:11434/api/version
```

#### Start LiteLLM Proxy

From the `config` directory:

```bash
cd config
litellm --config litellm.yaml
```

You should see output indicating LiteLLM is listening on port 8080.

#### Start Qdrant

From the `config` directory:

```bash
cd config
docker-compose up -d
```

Verify Qdrant is running:
```bash
curl http://localhost:6333/collections
```

### Step 3: Create Qdrant Collection

Create a collection for storing document embeddings:

```bash
curl -X PUT http://localhost:6333/collections/docs \
  -H "Content-Type: application/json" \
  -d '{
    "vectors": {
      "size": 768,
      "distance": "Cosine"
    }
  }'
```

**Note**: The vector size (768) should match your embedding model's output dimensions. For `nomic-embed-text`, use 768.

### Step 4: Build and Run the Application

Navigate to the solution directory:
```bash
cd dotnet/ai/local-rag-ollama-litellm
```

Restore dependencies:
```bash
dotnet restore
```

Build the solution:
```bash
dotnet build
```

Run the console application:

```bash
cd src/LocalRag.Console
dotnet run
```

### Step 5: Add Your Documents

Place your markdown files in `src/LocalRag.Console/documents/` and run the application again to index them.

## Project Structure

```
├── src/
│   ├── LocalRag.Core/           # Core library
│   │   ├── Clients/             # HTTP clients for LiteLLM, Qdrant
│   │   ├── Services/            # Indexing, chunking, RAG service
│   └── LocalRag.Console/        # Console application
│       └── documents/           # Place your .md files here
├── config/
│   ├── litellm.yaml            # LiteLLM configuration
│   └── docker-compose.yml      # Qdrant Docker setup
```

## Usage

### Ingesting Documents

Place your markdown files in `src/LocalRag.Console/documents/` and run the console application. It will:
1. Chunk each document into semantic segments
2. Generate embeddings for each chunk
3. Store vectors in Qdrant

### Querying

The RAG service:
1. Embeds your question
2. Retrieves top-k similar chunks from Qdrant
3. Composes a prompt with retrieved context
4. Generates a grounded answer via the local LLM

## Configuration

### LiteLLM (config/litellm.yaml)

Defines models and routing to Ollama:
- `nomic-embed-text`: Lightweight embedding model
- `qwen2.5`: General-purpose LLM for generation

### Vector Storage Options

- **Qdrant**: Fast vector search with HNSW indexing

## Architecture

### System Components

```
┌─────────────────────────────────────────────────────────────────┐
│                         RAG Pipeline                            │
└─────────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────────┐
│                      INDEXING PHASE (One-time)                    │
├───────────────────────────────────────────────────────────────────┤
│                                                                   │
│  1. Documents (*.md files)                                        │
│         │                                                         │
│         ▼                                                         │
│  2. Ingest.Chunk()                                                │
│     - Normalize text                                              │
│     - Split into 200-500 token chunks                             │
│     - 20-30% overlap                                              │
│         │                                                         │
│         ▼                                                         │
│  3. EmbeddingClient → LiteLLM → Ollama (nomic-embed-text)        │
│     - Convert text to 768-dim vectors                             │
│         │                                                         │
│         ▼                                                         │
│  4. QdrantClient.UpsertAsync()                                    │
│     - Store vectors + metadata                                    │
│     - HNSW indexing for fast search                               │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────────┐
│                   QUERY PHASE (Per question)                      │
├───────────────────────────────────────────────────────────────────┤
│                                                                   │
│  1. User Question                                                 │
│         │                                                         │
│         ▼                                                         │
│  2. EmbeddingClient.EmbedAsync()                                  │
│     - Convert question to vector                                  │
│         │                                                         │
│         ▼                                                         │
│  3. QdrantClient.SearchAsync()                                    │
│     - Cosine similarity search                                    │
│     - Return top-k chunks (k=4)                                   │
│         │                                                         │
│         ▼                                                         │
│  4. Compose Prompt                                                │
│     - System: "Answer with citations..."                          │
│     - User: "Context: [chunks]\n\nQuestion: ..."                  │
│         │                                                         │
│         ▼                                                         │
│  5. LLM Generation (qwen2.5 via LiteLLM)                          │
│     - Temperature: 0.2                                            │
│     - Grounded answer with citations                              │
│         │                                                         │
│         ▼                                                         │
│  6. Return Answer to User                                         │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘
```

### Component Diagram

```
┌────────────────────┐
│  LocalRag.Console  │
│   (Entry Point)    │
└──────────┬─────────┘
           │
           ▼
┌────────────────────────────────────────────────────┐
│              LocalRag.Core                         │
├────────────────────────────────────────────────────┤
│                                                    │
│  ┌─────────────┐  ┌──────────────┐               │
│  │  Indexer    │  │  RagService  │               │
│  └──────┬──────┘  └──────┬───────┘               │
│         │                 │                        │
│         ▼                 ▼                        │
│  ┌─────────────────────────────┐                  │
│  │        Ingest.Chunk()       │                  │
│  └─────────────────────────────┘                  │
│         │                                          │
│         ▼                                          │
│  ┌────────────────┐  ┌──────────────┐            │
│  │ EmbeddingClient│  │ QdrantClient │            │
│  └────────┬───────┘  └──────┬───────┘            │
│           │                  │                     │
│           │                  │                     │
└───────────┼──────────────────┼─────────────────────┘
            │                  │
            ▼                  ▼
   ┌────────────────┐  ┌─────────────┐
   │    LiteLLM     │  │   Qdrant    │
   │  (Port 8080)   │  │ (Port 6333) │
   └────────┬───────┘  └─────────────┘
            │
            ▼
   ┌────────────────┐
   │     Ollama     │
   │ (Port 11434)   │
   └────────────────┘
```

### Data Flow

#### Indexing Flow

1. **Input**: Markdown files from `documents/` folder
2. **Chunking**: Text split into semantic segments (200-500 tokens)
3. **Embedding**: Each chunk → 768-dimensional vector
4. **Storage**: Vector + metadata → Qdrant collection

#### Query Flow

1. **Input**: User question (string)
2. **Embedding**: Question → 768-dimensional vector
3. **Retrieval**: Top-k similar chunks from Qdrant (cosine similarity)
4. **Context Building**: Retrieved chunks formatted as context
5. **Generation**: LLM generates answer based on context
6. **Output**: Grounded answer with citations

### Technology Stack

- **.NET 8+**: Core framework
- **Ollama**: Local LLM runtime
  - `nomic-embed-text`: Embeddings (768-dim)
  - `qwen2.5`: Text generation
- **LiteLLM**: Unified API proxy
- **Qdrant**: Vector database (HNSW indexing)

### Performance Characteristics

- **Indexing**: ~500-1000ms per document (depends on size)
- **Query**: <500ms total
  - Embedding: ~50-100ms
  - Vector search: ~10-50ms
  - LLM generation: ~200-400ms
- **Vector dimension**: 768 (nomic-embed-text)
- **Chunk size**: 200-500 tokens
- **Retrieval**: Top-4 chunks by default

## Tips


## Troubleshooting

### Ollama Connection Issues

- Ensure Ollama is running: `ollama list`
- Check the base URL in `litellm.yaml` matches your Ollama installation
- On Windows, Ollama typically runs on `http://localhost:11434`

### LiteLLM Connection Issues

- Verify LiteLLM is running and listening on port 8080
- Check the LiteLLM logs for errors
- Ensure no other service is using port 8080

### Qdrant Connection Issues

- Verify Qdrant container is running: `docker ps`
- Check Qdrant logs: `docker logs qdrant`
- Ensure port 6333 is not blocked by firewall

### Collection Already Exists Error

If you need to recreate the collection:
```bash
curl -X DELETE http://localhost:6333/collections/docs
```

Then create it again with the PUT command from Step 3.

## Related Blog Post

For detailed explanation and architecture, see: [Local RAG with Ollama, LiteLLM, and Qdrant](https://gabrielmongeon.ca/en/2025/12/local-rag-ollama-litellm-qdrant/)

## License

See [LICENSE](../../../LICENSE) in the root of the repository.
