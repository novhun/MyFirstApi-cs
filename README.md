# MyFirstApi-cs

A minimal ASP.NET Core API for managing products, with SQLite persistence.

## Run the API

Install the .NET 10 SDK, then run these commands from the project directory:

```bash
dotnet restore
dotnet run --launch-profile http
```

The API listens at `http://localhost:5203`. On first startup, it creates `products.db` and the `Products` table in the project directory.

## Product endpoints

| Method | URL | Purpose |
| --- | --- | --- |
| `GET` | `/api/products` | List all products |
| `GET` | `/api/products/{id}` | Get one product |
| `POST` | `/api/products` | Create a product |
| `PUT` | `/api/products/{id}` | Replace a product's editable fields |
| `DELETE` | `/api/products/{id}` | Delete a product |

Create and update requests use this JSON shape:

```json
{
	"name": "Wireless Keyboard",
	"description": "Compact mechanical keyboard",
	"price": 79.99,
	"stockQuantity": 25
}
```

`name` is required (up to 200 characters), `description` is optional (up to 2000 characters), `price` must be greater than zero, and `stockQuantity` cannot be negative. Create returns `201 Created` with the new product URL. Requests for unknown IDs return `404 Not Found`.

## Try the requests

Open [MyFirstApi-cs.http](MyFirstApi-cs.http) in VS Code and select **Send Request** above a request. The file contains examples for listing, creating, fetching, updating, and deleting products.

When run with the `http` launch profile, the browser opens the interactive API reference at `http://localhost:5203/swagger`. The OpenAPI document is available at `http://localhost:5203/openapi/v1.json`.
