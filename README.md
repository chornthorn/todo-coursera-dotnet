# Todo API

A simple and comprehensive Todo API built with ASP.NET Core that provides full CRUD operations for managing todo items.

## Features

- Create, read, update, and delete todo items
- Filter todos by completion status, category, and priority
- Get todo statistics and analytics
- Basic authentication required for all endpoints
- Comprehensive validation and error handling
- Support for categories and priorities
- Due date management with validation

## Getting Started

### Prerequisites

- .NET 8.0 or later
- SQL Server (or SQL Server LocalDB)

### Running the API

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```
4. The API will be available at `http://localhost:5230`

### Authentication

All API endpoints require Basic Authentication:
- **Username**: `admin`
- **Password**: `admin123`
- **Base64 Encoded**: `YWRtaW46YWRtaW4xMjM=`

Include the authorization header in all requests:
```
Authorization: Basic YWRtaW46YWRtaW4xMjM=
```

## API Endpoints

### Base URL
```
http://localhost:5230/api/todo
```

### 1. Get All Todos
**GET** `/api/todo`

Retrieve all todo items with optional filtering.

**Query Parameters:**
- `isCompleted` (boolean, optional): Filter by completion status
- `category` (string, optional): Filter by category
- `priority` (integer, optional): Filter by priority level (1-5)

**Example:**
```bash
curl -X GET "http://localhost:5230/api/todo?isCompleted=false&priority=3" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 2. Get Todo by ID
**GET** `/api/todo/{id}`

Retrieve a specific todo item by its ID.

**Example:**
```bash
curl -X GET "http://localhost:5230/api/todo/1" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 3. Create New Todo
**POST** `/api/todo`

Create a new todo item.

**Request Body:**
```json
{
  "title": "Complete project documentation",
  "description": "Write comprehensive API documentation",
  "priority": 4,
  "category": "Work",
  "dueDate": "2024-12-31T23:59:59Z"
}
```

**Example:**
```bash
curl -X POST "http://localhost:5230/api/todo" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM=" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Complete project documentation",
    "description": "Write comprehensive API documentation",
    "priority": 4,
    "category": "Work",
    "dueDate": "2024-12-31T23:59:59Z"
  }'
```

### 4. Update Todo (Complete Replace)
**PUT** `/api/todo/{id}`

Update an existing todo item completely.

**Request Body:**
```json
{
  "title": "Updated Todo Title",
  "description": "Updated description",
  "isCompleted": true,
  "priority": 5,
  "category": "Updated",
  "dueDate": "2024-12-25T12:00:00Z"
}
```

### 5. Partial Update Todo
**PATCH** `/api/todo/{id}`

Partially update specific fields of a todo item.

**Request Body:**
```json
{
  "isCompleted": true,
  "priority": 5
}
```

### 6. Mark Todo as Completed
**PATCH** `/api/todo/{id}/complete`

Mark a specific todo item as completed.

**Example:**
```bash
curl -X PATCH "http://localhost:5230/api/todo/1/complete" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 7. Mark Todo as Incomplete
**PATCH** `/api/todo/{id}/incomplete`

Mark a specific todo item as incomplete.

**Example:**
```bash
curl -X PATCH "http://localhost:5230/api/todo/1/incomplete" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 8. Delete Specific Todo
**DELETE** `/api/todo/{id}`

Delete a specific todo item.

**Example:**
```bash
curl -X DELETE "http://localhost:5230/api/todo/1" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 9. Delete All Completed Todos
**DELETE** `/api/todo/completed`

Delete all completed todo items.

**Example:**
```bash
curl -X DELETE "http://localhost:5230/api/todo/completed" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 10. Get Todo Statistics
**GET** `/api/todo/stats`

Get comprehensive statistics about your todo items.

**Response:**
```json
{
  "totalTodos": 10,
  "completedTodos": 4,
  "pendingTodos": 6,
  "overdueTodos": 2,
  "completionRate": 40.0,
  "priorityBreakdown": {
    "Priority1": 2,
    "Priority2": 3,
    "Priority3": 3,
    "Priority4": 1,
    "Priority5": 1
  },
  "categoryBreakdown": {
    "Work": 5,
    "Personal": 3,
    "Uncategorized": 2
  }
}
```

## Data Model

### Todo Item Structure
```json
{
  "id": 1,
  "title": "Sample Todo",
  "description": "This is a sample todo item",
  "isCompleted": false,
  "priority": 3,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": "2024-01-01T10:00:00Z",
  "dueDate": "2024-12-31T23:59:59Z",
  "category": "Work"
}
```

### Field Validation
- **title**: Required, 1-200 characters
- **description**: Optional, max 1000 characters
- **priority**: Integer between 1 (low) and 5 (high)
- **category**: Optional, max 50 characters
- **dueDate**: Optional, cannot be in the past

## HTTP Status Codes

- `200 OK`: Request successful
- `201 Created`: Resource created successfully
- `204 No Content`: Resource deleted successfully
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `404 Not Found`: Resource not found

## Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Title is required"]
  }
}
```

## Documentation

The API includes Swagger/OpenAPI documentation available at:
- Swagger UI: `http://localhost:5230/swagger`
- ReDoc: `http://localhost:5230/api-docs`

## Testing

Use the included `TodoAPI.http` file with your HTTP client (like REST Client in VS Code) to test all endpoints with pre-configured requests.
