# Api Specification for Member

## Get Members

Request:
- Method: GET
- Endpoint: `/api/members`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json
- Query:
  - memberNumber: null
  - name: null
  - address: null
  - email: null
  - phone: null
  - page: 1
  - size: 15

Response:

```json
[
  {
    "id": "string",
    "memberNumber": "string",
    "name": "string",
    "address": "string",
    "email": "string",
    "phones": [
      "string"
    ],
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
]
```

## Get Member

Request:
- Method: GET
- Endpoint: `/api/members/{memberId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json

Response:

```json
{
  "id": "string",
  "memberNumber": "string",
  "name": "string",
  "address": "string",
  "email": "string",
  "phones": [
    "string"
  ],
  "imageUrl": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Create Member

Request:
- Method: POST
- Endpoint: `/api/members`
- Header:
  - Authorization: Bearer <token>
  - Accept: multipart/form-data
  - Content-Type: multipart/form-data

Form:

- memberNumber: string,
- name: string,
- address: string,
- email: string,
- phones: array of string,
- image: file

Response:

```json
{
  "id": "string",
  "memberNumber": "string",
  "name": "string",
  "address": "string",
  "email": "string",
  "phones": [
    "string"
  ],
  "imageUrl": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Update Member

Request:
- Method: PUT
- Endpoint: `/api/members/{memberId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: multipart/form-data
  - Content-Type: multipart/form-data

Form:

- memberNumber: string,
- name: string,
- address: string,
- email: string,
- phones: array of string,
- image: file

Response:

```json
{
  "id": "string",
  "memberNumber": "string",
  "name": "string",
  "address": "string",
  "email": "string",
  "phones": [
    "string"
  ],
  "imageUrl": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Delete Member

Request:
- Method: DELETE
- Endpoint: `/api/members/{memberId}`
- Header:
  - Authorization: Bearer <token>
  - Accept: application/json

Response:

204 (No Content)