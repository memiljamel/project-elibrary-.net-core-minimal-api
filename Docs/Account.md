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
  "accessToken": "string"
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