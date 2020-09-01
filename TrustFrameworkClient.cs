using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AzureAdB2c.Utility
{
    public class TrustFrameworkClient
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string tenantId;

        public TrustFrameworkClient(string clientId, string clientSecret, string tenantId)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.tenantId = tenantId;
        }

        public async Task<DateTimeOffset> GetCertificateExpiration(string keySetId) {
            var client = CreateClient();

            var response = await client.TrustFramework.KeySets
                .Request()
                .GetAsync();

            var key = response.CurrentPage.First(k => k.Id == keySetId).Keys.First();
            var certificateString = key.X5c.First();

            var certificate = new X509Certificate2(Convert.FromBase64String(certificateString));
            var expirationDate = certificate.GetExpirationDateString();

            return DateTimeOffset.Parse(expirationDate);
        }

        private GraphServiceClient CreateClient() {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            var authProvider = new ClientCredentialProvider(confidentialClientApplication);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            return graphClient;
        }
    }
}