# Transaction System API

A REST API for managing credit cards and transactions with automatic currency conversion via the [US Treasury Fiscal Data API](https://fiscaldata.treasury.gov/).

## Stack

- .NET 8 / ASP.NET Core
- PostgreSQL + Entity Framework Core
- Docker Compose

---

## Setup

### Option 1 — Docker (recommended)

Requires [Docker Desktop](https://www.docker.com/products/docker-desktop/).

Run the following command at &lt;path&gt;/TransactionSystemApi/TransactionSystemApi
```bash
docker compose up --build
```

This starts both the API and a PostgreSQL instance. Database migrations run automatically on startup.

- API: `http://localhost:8080`
- Swagger UI: `http://localhost:8080/swagger`

### Option 2 — Run locally

**Prerequisites:**
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL running on `localhost:5432`

**Steps:**

1. Update the connection string in `TransactionSystemApi/appsettings.json` if needed:
    ```json
    "ConnectionStrings": {
      "Default": "Host=localhost;Port=5432;Database=transactionsSystemDB;Username=postgres;Password=postgres"
    }
    ```

2. Run the API:
    ```bash
    cd TransactionSystemApi
    dotnet run
    ```

Migrations are applied automatically on startup. Swagger UI is available at `http://localhost:5000/swagger` in development.

---

## API Endpoints

### Cards

#### `POST /card/create`
Creates a new credit card.

**Request body:**
```json
{
  "creditLimit": 5000.00
}
```

**Validation:**
- `creditLimit` must be greater than `0`

**Response `200 OK`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "creditLimit": 5000.00
}
```

---

#### `GET /card/{cardId}/balance?currency={currency}`
Returns the remaining balance on a card, converted to the specified currency using the latest available exchange rate.

**Path parameter:**
- `cardId` — UUID of the card

**Query parameter:**
- `currency` — currency name as it appears in the Treasury API (e.g. `Euro`, `Canadian Dollar`, `Japanese Yen`)

**Response `200 OK`:**
```json
920.50
```

**Error responses:**
- `404 Not Found` — card does not exist
- `400 Bad Request` — no exchange rate available for the given currency

---

### Transactions

#### `POST /transaction/create`
Records a purchase transaction against a card.

**Request body:**
```json
{
  "description": "Grocery shopping",
  "date": "2025-06-01",
  "cardId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 120.00
}
```

**Validation:**
- `amount` must be greater than `0`
- `description`, `date`, and `cardId` are required

**Response `200 OK`:**
```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "cardId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2025-06-01",
  "amount": 120.00,
  "description": "Grocery shopping"
}
```

---

#### `GET /transaction/{transactionId}/get?currency={currency}`
Retrieves a transaction and returns the amount converted to the specified currency using the exchange rate closest to the transaction date (within 6 months).

**Path parameter:**
- `transactionId` — UUID of the transaction

**Query parameter:**
- `currency` — currency name (e.g. `Euro`, `Canadian Dollar`, `Japanese Yen`)

**Response `200 OK`:**
```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "cardId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2025-06-01",
  "amount": 120.00,
  "description": "Grocery shopping",
  "currency": "Euro",
  "exchangeRate": 0.92,
  "convertedAmount": 110.40
}
```

**Error responses:**
- `404 Not Found` — transaction does not exist
- `400 Bad Request` — no exchange rate available within 6 months of the transaction date

---

## Running Tests

```bash
dotnet test
```

The test suite contains 30 tests:
- **Unit tests** — services and controllers tested in isolation with mocked dependencies
- **Integration tests** — full HTTP pipeline with an in-memory SQLite database, covering routing, validation, and error handling middleware
