# Azure AD B2C - How To Check Certificate Expiration

Azure AD B2C allows for custom policies to have certificates uploaded to what's called a "KeySet". However, other than manually confirming the certificate's expiration date, there's really no easily apparent way to automate this.


Yet another C# example.

## Solution overview

![diagram](./img/diagram.png)

Using Azure Functions, you'll retrieve an Azure AD B2C policy certificate's expiration date using the [Microsoft Graph SDK (beta)](https://docs.microsoft.com/en-us/graph/sdks/use-beta?context=graph%2Fapi%2F1.0&tabs=CS), fetching the [KeySet information](https://docs.microsoft.com/en-us/graph/api/trustframeworkkeyset-get?view=graph-rest-beta&tabs=http).

### Responding (the scheduled caller)

In this example, I setup a scheduled Logic App that will execute on an interval:

1. Store an array of policy key ids in a Logic App variable (set it to what you want or you could retrieve this list dynamically)
2. Execute the Function, get the result, and take conditional action
3. In this example, I triggered sending an SMS to be sent

You can trigger anything if you don't want a simple SMS or email to be sent. You can even cause another process to be triggered to rotate the key value in B2C.


# Getting Started

## BC2 configuration

1.  Create an [app registration in the B2C tenant](https://docs.microsoft.com/en-us/graph/auth-v2-service)
2. Give it **Application Permission** of [TrustFrameworkKeySet.Read.All](https://docs.microsoft.com/en-us/graph/api/trustframeworkkeyset-get?view=graph-rest-beta&tabs=http)

You'll need the following "parameters" from the app registration:

-   Client ID of the app
-   Client secret
-   the tenant ID of the B2C tenant (format: mydomain.onmicrosoft.com)

## Deploy resources

Follow these steps to get this setup and running. First, open cloud shell in Bash.

```bash
git clone --depth 1 https://github.com/kevinhillinger/check-b2c-cert-expiration.git 
cd check-b2c-cert-expiration

./scripts/deploy.sh
```

### Resources that get deployed

In ```./scripts/deploy.sh```, the following gets deployed:

-   Resource Group
-   Function App (on demand, linux)
    -   Storage account for the function instance
    -   Application Insights instance for logging
    -   values in the FunctionApp's application settings should get set
-   Logic App

## Running the serverless function locally

[Install the Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=linux%2Ccsharp%2Cbash#install-the-azure-functions-core-tools)

```
cd src/Functions
func start --build
```

**App Settings:**

```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "B2C_CLIENT_ID": "<client id, e.g. 845cea86-4a21-406a-b5ef-7abb75b8b5f9>",
        "B2C_CLIENT_SECRET": "<client secret>",
        "B2C_TENANT_ID": "<the b2c domain, e.g. mydomain.onmicrosoft.com>"
    }
}
```


## Logic App - Scheduled check of a list of certificates 

![logic app flow](./img/logicapp-flow.png)

Using a logic app found in ```src/Logic```, you can deploy this definition as an example of how to schedule the work to check a list of certificates.

There are placeholder values you'll need to update in the logic app definition before deploying.

### Example SMS message

<img src="./img/sms-example.png" width="250" />

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
