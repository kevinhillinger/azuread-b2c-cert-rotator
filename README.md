# check-b2c-cert-expiration
Using Azure Functions, check if an Azure AD B2C policy certificate's expiration date

## Sample POST to the Azure Function

```
POST http://localhost:7071/api/GetCertificateExpiration

{
    "policyKeyId": "B2C_1A_Certificate"
}
```

Example response:

```
{
    "expired": true,
    "hoursToExpiration": -3077,
    "value": "2020-04-26T11:07:01-04:00"
}
```