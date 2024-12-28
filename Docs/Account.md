# Api Specification for Account

## Login

Request:

- Method: POST
- Endpoint: `/api/account/login`
- Header:
    - Accept: application/json
    - Content-Type: application/json

Body:

```json
{
  "username": "string",
  "password": "string"
}
```

Response:

```json
{
  "code": "int",
  "status": "string",
  "data": {
    "accessToken": "string"
  }
}
```

## Get Account

Request:

- Method: GET
- Endpoint: `/api/account/current`
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
    "username": "string",
    "name": "string",
    "staffNumber": "string",
    "accessLevel": "enum",
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```

## Update Current

Request:

- Method: PUT
- Endpoint: `/api/account/current`
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
  "code": "int",
  "status": "string",
  "data": {
    "id": "string",
    "username": "string",
    "name": "string",
    "staffNumber": "string",
    "accessLevel": "enum",
    "imageUrl": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
}
```
