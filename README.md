# AspireMQDemo

This repository contains a sample message-queue-based architecture built with .NET 9 and C# 13. The goal is to demonstrate how to send requests coming from a Web API to a background worker service through a message queue to achieve a loosely coupled and scalable system.

## Tech Stack

- .NET 9
- C# 13
- ASP.NET Core Web API
- Background Worker Service (Worker Service template)
- Messaging / Queue abstraction (custom `IQueuePublisher` / `QueueMessageModel`)
- .NET Aspire host (`AspireHost` project)

## Architecture Overview

The solution consists of 4 main projects:

- `AspireHost`
  - Aspire host project.
  - Orchestrates and configures applications such as the Web API and Worker.

- `AspireMQDemoWebApi`
  - Exposes product CRUD operations via `api/products` endpoints.
  - Instead of writing directly to the database on create, it publishes a `QueueMessageModel` to a queue via `IQueuePublisher`.
  - Uses the `OperationType` enum (e.g. `Create`) to indicate which operation should be executed.

- `AspireMQDemoWorker`
  - Background worker application that consumes messages from the queue.
  - Reads `QueueMessageModel` messages and, based on `OperationType`, performs the corresponding operation (`Create`, `Update`, `Delete`, etc.) through `IProductService`.
  - Encapsulates the actual side effects (such as persisting products) so that the API stays thin and responsive.

- `Shared`
  - Contains types shared across projects.
  - Examples:
    - `Product` entity
    - `OperationType` enum
    - `QueueMessageModel` DTO

## Main Flow

1. A client sends a request to create a product using `POST /api/products`.
2. The Web API maps the incoming `Product` body to a `QueueMessageModel` and publishes it to the queue with `OperationType.Create`.
3. The Worker listens to the queue, receives the message, and uses `IProductService` to persist the product.
4. The same pattern can be used for update/delete operations if desired.
5. This separates the Web API from the background processing logic and makes the system more resilient under heavy load.

## Running the Solution

> Note: The steps below are indicative. For actual configuration/commands, see `appsettings.json` and `Program.cs` in each project.

1. Configure connection strings and queue settings in `AspireHost` and each application's `appsettings.json`.
2. Restore and build the solution:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Start the Aspire host (or use your orchestration tool to start all components). This should bring up:
   - `AspireMQDemoWebApi`
   - `AspireMQDemoWorker`
4. Once the Web API is running, you can test it using the sample HTTP requests below.

## Sample HTTP Requests

### Create Product (sends message to the queue)

```http
POST /api/products HTTP/1.1
Content-Type: application/json

{
  "name": "Test Product",
  "price": 99.99
}
```

### Get All Products

```http
GET /api/products HTTP/1.1
```

### Get Product By Id

```http
GET /api/products/{id} HTTP/1.1
```

### Update Product

```http
PUT /api/products/{id} HTTP/1.1
Content-Type: application/json

{
  "name": "Updated Product",
  "price": 149.99
}
```

### Delete Product

```http
DELETE /api/products/{id} HTTP/1.1
```

## Development Notes

- The solution targets .NET 9; ensure the correct SDK is installed.
- This project can be extended with CQRS, outbox pattern, retry policies, dead-letter queues, etc.
- Shared models live in the `Shared` project and are reused by both Web API and Worker.

## Contributing

- Open a GitHub Issue for bugs or feature requests.
- Before opening a pull request, build the solution and manually test the basic flows (create, get, update, delete).