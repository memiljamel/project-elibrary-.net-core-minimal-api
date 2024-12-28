# Api Specification for Loan

## Get Loans

Request:

- Method: GET
- Endpoint: `/api/loans`
- Header:
    - Authorization: Bearer <token>
    - Accept: application/json
- Query:
    - loanDate: null
    - returnDate: null
    - bookTitle: null
    - memberNumber: null
    - isReturned: null
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
      "loanDate": "date",
      "returnDate": "date",
      "bookTitle": "string",
      "memberNumber": "string",
      "isReturned": "boolean",
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

## Get Loan

Request:

- Method: GET
- Endpoint: `/api/loans/{loanId}`
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
    "loanDate": "date",
    "returnDate": "date",
    "bookTitle": "string",
    "memberNumber": "string",
    "isReturned": "boolean",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Create Loan

Request:

- Method: POST
- Endpoint: `/api/loans`
- Header:
    - Authorization: Bearer <token>
    - Accept: application/json
    - Content-Type: application/json

Body:

```json
{
  "loanDate": "date",
  "bookId": "string",
  "memberId": "string"
}
```

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": {
    "id": "string",
    "loanDate": "date",
    "returnDate": "date",
    "bookTitle": "string",
    "memberNumber": "string",
    "isReturned": "boolean",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Update Loan

Request:

- Method: PUT
- Endpoint: `/api/loans/{loanId}`
- Header:
    - Authorization: Bearer <token>
    - Accept: application/json
    - Content-Type: application/json

Body:

```json
{
  "returnDate": "date",
  "bookId": "string",
  "memberId": "string"
}
```

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": {
    "id": "string",
    "loanDate": "date",
    "returnDate": "date",
    "bookTitle": "string",
    "memberNumber": "string",
    "isReturned": "boolean",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Delete Loan

Request:

- Method: DELETE
- Endpoint: `/api/loans/{loanId}`
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
