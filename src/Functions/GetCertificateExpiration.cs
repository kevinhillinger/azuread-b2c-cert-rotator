using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureAdB2c.Utility
{
    public class Response
    {
        public bool Expired { get; set; }
        public int HoursToExpiration { get; set; }
        public DateTimeOffset? Value { get; set; }
    }

    public static class GetCertificateExpiration
    {
        const int HoursBeforeCertificateConsideredExpired = 12;

        [FunctionName("GetCertificateExpiration")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            // ex value 'B2C_1A_ClientCertificate'
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string policyKeyId = data?.policyKeyId;

            log.LogInformation("Triggered request to verify certificate expiration for B2C Policy: {0}", policyKeyId);

            var response = await GetResponse(policyKeyId);
            log.LogInformation("Result: {0}", JsonConvert.SerializeObject(response));

            return new OkObjectResult(response);
        }

        private static async Task<Response> GetResponse(string policyKeyId)
        {
            var response = new Response();

            if (!string.IsNullOrEmpty(policyKeyId))
            {
                var client = new TrustFrameworkClient(
                    clientId: Environment.GetEnvironmentVariable("B2C_CLIENT_ID"),
                    clientSecret: Environment.GetEnvironmentVariable("B2C_CLIENT_SECRET"),
                    tenantId: Environment.GetEnvironmentVariable("B2C_TENANT_ID"));

                DateTimeOffset expiration = await client.GetCertificateExpiration(policyKeyId);

                response.Value = expiration;
                response.HoursToExpiration = (int)expiration.Subtract(DateTimeOffset.UtcNow).TotalHours;
                response.Expired = response.HoursToExpiration < HoursBeforeCertificateConsideredExpired;
            }
            else
            {
                response.Expired = true;
                response.HoursToExpiration = -1;
                response.Value = null;
            }

            return response;
        }
    }
}
