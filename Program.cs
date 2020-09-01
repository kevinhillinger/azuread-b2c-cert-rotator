using System;
using System.Threading.Tasks;

namespace AzureAdB2c.Utility
{
    class Program
    {
        // the client id and secret are the values from the app registration
        // that you will create IN the B2C tenant.

        // https://docs.microsoft.com/en-us/graph/auth-v2-service

        // information on REST API for getting key sets data and Application permission to read the keys, e.g. TrustFrameworkKeySet.Read.All
        // https://docs.microsoft.com/en-us/graph/api/trustframeworkkeyset-get?view=graph-rest-beta&tabs=http

        const string clientId = "845cea86-4a21-406a-b5ef-7abb75b8b5f9";
        const string clientSecret = "<the client secret created in the app registration>";
        const string tenantId = "<tenant name>.onmicrosoft.com";

        // this is the name/id of the policy key that holds the certificate
        const string policyKeyId = "B2C_1A_ApiClientCertificate";

        static async Task Main(string[] args)
        {
            var client = new TrustFrameworkClient(clientId, clientSecret, tenantId);
            var expiration = await client.GetCertificateExpiration(policyKeyId);
            var dateTime = expiration.DateTime;

            // print out the date/time of the certificate
            Console.WriteLine($"Expiration Date: {dateTime.ToLongDateString()} {dateTime.ToLongTimeString()}");
        }
    }
}
