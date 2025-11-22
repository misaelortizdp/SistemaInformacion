# API Documentation - CashFlowSystem

## Base URL
```
Development: https://localhost:7000
Production: https://your-domain.com
```

## Authentication

All endpoints except `/api/auth/*` and `/api/health` require JWT authentication.

### Headers
```
Authorization: Bearer {your-jwt-token}
Content-Type: application/json
```

---

## 🔐 Authentication Endpoints

### POST /api/auth/register
Register a new user account.

**Request Body:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "confirmPassword": "string"
}
```

**Response:** `201 Created`
```json
{
  "userId": "guid",
  "username": "string",
  "email": "string",
  "role": "string",
  "token": "string",
  "expiresAt": "datetime"
}
```

### POST /api/auth/login
Login with email and password.

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Response:** `200 OK`
```json
{
  "userId": "guid",
  "username": "string",
  "email": "string",
  "role": "string",
  "token": "string",
  "expiresAt": "datetime"
}
```

---

## 💰 Transactions Endpoints

### GET /api/transactions
Get all transactions with optional filters.

**Query Parameters:**
- `userId` (optional): Filter by user ID
- `startDate` (optional): Filter by start date
- `endDate` (optional): Filter by end date
- `type` (optional): "Income" or "Expense"

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "date": "datetime",
    "amount": 100.00,
    "description": "string",
    "type": "Income",
    "categoryId": "guid",
    "categoryName": "string",
    "paymentMethodId": "guid",
    "paymentMethodName": "string",
    "referenceNumber": "string",
    "notes": "string"
  }
]
```

### GET /api/transactions/{id}
Get a single transaction by ID.

**Response:** `200 OK`
```json
{
  "id": "guid",
  "date": "datetime",
  "amount": 100.00,
  "description": "string",
  "type": "Income",
  "categoryId": "guid",
  "categoryName": "string",
  "paymentMethodId": "guid",
  "paymentMethodName": "string",
  "referenceNumber": "string",
  "notes": "string"
}
```

### POST /api/transactions
Create a new transaction.

**Request Body:**
```json
{
  "date": "datetime",
  "amount": 100.00,
  "description": "string",
  "type": "Income",
  "categoryId": "guid",
  "paymentMethodId": "guid",
  "userId": "guid",
  "referenceNumber": "string",
  "notes": "string"
}
```

**Response:** `201 Created`

### PUT /api/transactions/{id}
Update an existing transaction.

**Request Body:**
```json
{
  "id": "guid",
  "date": "datetime",
  "amount": 100.00,
  "description": "string",
  "categoryId": "guid",
  "paymentMethodId": "guid",
  "referenceNumber": "string",
  "notes": "string"
}
```

**Response:** `200 OK`

### DELETE /api/transactions/{id}
Delete a transaction (soft delete).

**Response:** `204 No Content`

---

## 📂 Categories Endpoints

### GET /api/categories
Get all active categories.

**Query Parameters:**
- `type` (optional): "Income" or "Expense"

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "string",
    "type": "Income",
    "color": "#4CAF50",
    "icon": "currency-usd",
    "isActive": true
  }
]
```

---

## 💳 Payment Methods Endpoints

### GET /api/paymentmethods
Get all active payment methods.

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Efectivo",
    "type": "Cash",
    "isActive": true
  }
]
```

---

## 📊 Dashboard Endpoint

### GET /api/dashboard
Get dashboard data for a date range.

**Query Parameters:**
- `startDate` (optional): Default to first day of current month
- `endDate` (optional): Default to today
- `userId` (optional): Filter by user

**Response:** `200 OK`
```json
{
  "currentBalance": 5000.00,
  "totalIncome": 10000.00,
  "totalExpense": 5000.00,
  "netFlow": 5000.00,
  "recentTransactions": [...],
  "incomeByCategory": {
    "Ventas": 8000.00,
    "Servicios": 2000.00
  },
  "expenseByCategory": {
    "Compras": 3000.00,
    "Sueldos": 2000.00
  },
  "dailyFlow": [
    {
      "date": "2024-01-15",
      "income": 500.00,
      "expense": 200.00,
      "balance": 300.00
    }
  ]
}
```

---

## 📈 Reports Endpoint

### GET /api/reports
Generate a cash flow report.

**Query Parameters:**
- `startDate` (required): Report start date
- `endDate` (required): Report end date
- `userId` (optional): Filter by user
- `type` (optional): "Income" or "Expense"
- `categoryId` (optional): Filter by category

**Response:** `200 OK`
```json
{
  "title": "Cash Flow Report",
  "startDate": "2024-01-01",
  "endDate": "2024-01-31",
  "generatedAt": "2024-01-31T23:59:59Z",
  "summary": {
    "totalIncome": 10000.00,
    "totalExpense": 5000.00,
    "netFlow": 5000.00,
    "transactionCount": 50,
    "averageTransaction": 300.00
  },
  "transactions": [...],
  "categoryBreakdown": {
    "Ventas": 8000.00,
    "Servicios": 2000.00
  }
}
```

---

## 💵 Cash Register Endpoints

### GET /api/cashregister/open/{userId}
Get the open cash register for a user.

**Response:** `200 OK`
```json
{
  "id": "guid",
  "openingDate": "datetime",
  "closingDate": null,
  "openingBalance": 1000.00,
  "closingBalance": null,
  "expectedBalance": null,
  "difference": null,
  "status": "Open",
  "notes": "string",
  "userId": "guid"
}
```

### POST /api/cashregister/open
Open a new cash register.

**Request Body:**
```json
{
  "userId": "guid",
  "openingBalance": 1000.00,
  "notes": "string"
}
```

**Response:** `200 OK`

### POST /api/cashregister/close/{id}
Close a cash register.

**Request Body:**
```json
{
  "id": "guid",
  "closingBalance": 5000.00,
  "notes": "string"
}
```

**Response:** `200 OK`
```json
{
  "id": "guid",
  "openingDate": "datetime",
  "closingDate": "datetime",
  "openingBalance": 1000.00,
  "closingBalance": 5000.00,
  "expectedBalance": 4800.00,
  "difference": 200.00,
  "status": "Closed",
  "notes": "string",
  "userId": "guid"
}
```

---

## 🏥 Health Check Endpoint

### GET /api/health
Check API health status.

**Response:** `200 OK`
```json
{
  "status": "Healthy",
  "timestamp": "datetime",
  "version": "1.0.0",
  "service": "CashFlow System API"
}
```

---

## Error Responses

All endpoints may return the following error responses:

### 400 Bad Request
```json
{
  "message": "Error description"
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid email or password"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred"
}
```

---

## Rate Limiting

Currently no rate limiting is implemented. Consider adding in production.

## Versioning

Current version: v1

Future versions will be accessible via `/api/v2/...`

---

## Testing with Swagger

Access Swagger UI at: `https://localhost:7000`

Swagger provides interactive API documentation and testing capabilities.

### Using JWT in Swagger:
1. Login via `/api/auth/login`
2. Copy the `token` from the response
3. Click the "Authorize" button in Swagger UI
4. Enter: `Bearer {your-token}`
5. Click "Authorize"
6. Now you can test all protected endpoints
