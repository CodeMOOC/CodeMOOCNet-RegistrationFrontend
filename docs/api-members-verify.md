# Membership verification API

Simple private API to check membership status based on e-mail.

`POST https://dev.codemooc.net/api/members/verify?email=EMAILADDRESS`

Requests _require_ a `Authorization` header with [Basic authentication information](https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication).

Response format:

```json
{
  "id": 123,
  "primaryMail": "EMAILADDRESS",
  "registeredOn": "2019-01-07T11:10:14",
  "isMember": true,
  "memberships": {
    "2019": {
      "issuedOn": "2019-01-11T14:50:52"
    }
  }
}
```

| Attribute | Type | Description |
| --- | --- | --- |
| id | Numeric | Unique ID of the user (internal in CodeMOOC). |
| primaryMail | String | Primary e-mail address, may not be equal to the input e-mail. |
| registeredOn | UTC Date | Timestamp of registration on CodeMOOC. |
| isMember | Boolean | Whether the user is currently a member. |
| memberships | Object | Map of membership information by year. |
