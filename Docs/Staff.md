# Api Specification for Staff

## Get Staffs

Request:
- Method: GET
- Endpoint: `/api/staffs`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json
- Query:
  - username: null
  - name: null
  - staffNumber: null
  - accessLevel: null
  - page: 1
  - size: 15

Response:

```json
[
  {
    "id": "string",
    "username": "string",
    "name": "string",
    "staffNumber": "string",
    "accessLevel": "enum",
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
]
```

## Get Staff

Request:
- Method: GET
- Endpoint: `/api/staffs/{staffId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json

Response:

```json
  {
  "id": "string",
  "username": "string",
  "name": "string",
  "staffNumber": "string",
  "accessLevel": "enum",
  "imageUrl": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Create Staff

Request:
- Method: POST
- Endpoint: `/api/staffs`
- Header:
  - Authorization: Bearer <token>
  - Accept: multipart/form-data
  - Content-Type: multipart/form-data

Form:

- username: string,
- password: string,
- passwordConfirmation: string,
- name: string,
- staffNumber: string,
- accessLevel: enum,
- image: file

Response:

```json
{
  "id": "string",
  "username": "string",
  "name": "string",
  "staffNumber": "string",
  "accessLevel": "enum",
  "imageUrl": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Update Staff

Request:
- Method: PUT
- Endpoint: `/api/staffs/{staffId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: multipart/form-data
  - Content-Type: multipart/form-data

Form:

- username: string,
- password: string,
- passwordConfirmation: string,
- name: string,
- staffNumber: string,
- accessLevel: enum,
- image: file

Response:

```json
{
  "id": "string",
  "username": "string",
  "name": "string",
  "staffNumber": "string",
  "accessLevel": "enum",
  "imageUrl": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Delete Staff

Request:
- Method: DELETE
- Endpoint: `/api/staffs/{staffId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json

Response:

204 (No Content)