# Api Specification for Book

## Get Books

Request:

- Method: GET
- Endpoint: `/api/books`
- Header:
    - Authorization: Bearer <token>
    - Accept: application/json
- Query:
    - title: null
    - category: null
    - publisher: null
    - quantity: null
    - authors: null
    - page: 1
    - size: 15

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": [
    {
      "id": "string",
      "title": "string",
      "category": "enum",
      "publisher": "string",
      "quantity": "int",
      "authors": [
        "string"
      ],
      "imageUrl": "string",
      "createdAt": "datetime",
      "updatedAt": "datetime"
    }
  ],
  "meta": {
    "currentPage": "int",
    "perPage": "int",
    "total": "int",
    "totalPage": "int"
  }
}
```

## Get Book

Request:

- Method: GET
- Endpoint: `/api/books/{bookId}`
- Header:
    - Authorization: Bearer <token>
    - Accept: application/json

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": {
    "id": "string",
    "title": "string",
    "category": "enum",
    "publisher": "string",
    "quantity": "int",
    "authors": [
      "string"
    ],
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Create Book

Request:

- Method: POST
- Endpoint: `/api/books`
- Header:
    - Authorization: Bearer <token>
    - Accept: multipart/form-data
    - Content-Type: multipart/form-data

Form:

- title: string,
- category: enum,
- publisher: string,
- quantity: int,
- authorIds: array of string,
- image: file

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": {
    "id": "string",
    "title": "string",
    "category": "enum",
    "publisher": "string",
    "quantity": "int",
    "authors": [
      "string"
    ],
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Update Book

Request:

- Method: PUT
- Endpoint: `/api/books/{bookId}`
- Header:
    - Authorization: Bearer <token>
    - Accept: multipart/form-data
    - Content-Type: multipart/form-data

Form:

- title: string,
- category: enum,
- publisher: string,
- quantity: int,
- authorIds: array of string,
- image: file

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": {
    "id": "string",
    "title": "string",
    "category": "enum",
    "publisher": "string",
    "quantity": "int",
    "authors": [
      "string"
    ],
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Delete Book

Request:

- Method: DELETE
- Endpoint: `/api/books/{bookId}`
- Header:
    - Authorization: Bearer <token>
    - Accept: application/json

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": null
}
```