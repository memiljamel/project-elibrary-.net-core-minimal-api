﻿# Api Specification for Author

## Get Authors

Request:
- Method: GET
- Endpoint: `/api/authors`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json
- Query:
  - name: null
  - email: null
  - bookCount: null
  - page: 1
  - size: 15

Response:

```json
[
  {
    "id": "string",
    "name": "string",
    "email": "string",
    "bookCount": "int",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
]
```

## Get Author

Request:
- Method: GET
- Endpoint: `/api/authors/{authorId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json

Response:

```json
{
  "id": "string",
  "name": "string",
  "email": "string",
  "bookCount": "int",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Create Author

Request:
- Method: POST
- Endpoint: `/api/authors`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json
  - Content-Type: application/json

Body:

```json
{
  "name": "string",
  "email": "string"
}
```

Response:

```json
{
  "id": "string",
  "name": "string",
  "email": "string",
  "bookCount": "int",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Update Author

Request:
- Method: PUT
- Endpoint: `/api/authors/{authorId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json
  - Content-Type: application/json

Body:

```json
{
  "name": "string",
  "email": "string"
}
```

Response:

```json
{
  "id": "string",
  "name": "string",
  "email": "string",
  "bookCount": "int",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Delete Author

Request:
- Method: DELETE
- Endpoint: `/api/authors/{authorId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json

Response:

204 (No Content)