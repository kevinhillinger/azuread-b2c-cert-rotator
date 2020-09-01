# check-b2c-cert-expiration
Using Azure Functions, check if an Azure AD B2C policy certificate's expiration date

## Setup

1.  Create an [app registration in the B2C tenant](https://docs.microsoft.com/en-us/graph/auth-v2-service)
2. Give it **Application Permission** of [TrustFrameworkKeySet.Read.All](https://docs.microsoft.com/en-us/graph/api/trustframeworkkeyset-get?view=graph-rest-beta&tabs=http)
3. Set the values in the FunctionApp's application settings via the CLI or portal
4. Call the HTTP function with a POST request and get the result. 

**App Settings:**

```
"B2C_CLIENT_ID":              "<client id, e.g. 845cea86-4a21-406a-b5ef-7abb75b8b5f9>"
"B2C_CLIENT_SECRET":    "<client secret>"
"B2C_TENANT_ID":            "<the b2c domain, e.g. mydomain.onmicrosoft.com>"
```

## Responding to the result

1.  You can use it as a webhook to get the result. Change it to a GET request
2. The FunctionApp can be changed to a scheduled trigger, log to Application Insights, then setup a log alert based on the JSON that it logs

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