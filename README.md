# check-b2c-cert-expiration
Using Azure Functions, check if an Azure AD B2C policy certificate's expiration date

## Sample POST to the Azure Function

```
POST http://localhost:7071/api/GetCertificateExpiration

{
    "policyKeyId": "B2C_1A_Certificate"
}
```