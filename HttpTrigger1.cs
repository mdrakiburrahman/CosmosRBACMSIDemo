using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Azure.Identity;


namespace Company.Function
{
    public class Review
    {
        public string Id { get; set; }
        public string username { get; set; }
        public string product { get; set; }
        public string review { get; set; }
        public string rating { get; set; }
        public string reviewDate { get; set; }
        public string documentType { get; set; }
    }
    public static class HttpTrigger1
    {
        [FunctionName("GetReview")]
        public static async Task<IActionResult> GetReview(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,ILogger log)
        {
            try
            {
                ManagedIdentityCredential tokenCredential = new ManagedIdentityCredential();
                CosmosClient client = new CosmosClient("https://aiacosmosrbacdemo.documents.azure.com:443/", tokenCredential);
                Container container = client.GetContainer("eCommerceSampleDB", "UserReviews");
                ItemResponse<Review> res = await container.ReadItemAsync<Review>("c188bdfc-2cbb-44c2-b7db-c13b6b71079b", new PartitionKey("Necklace"));
                return new OkObjectResult(JsonConvert.SerializeObject(res.Resource));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }
        }

        [FunctionName("CreateReview")]
        public static async Task<IActionResult> CreateReview(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                ManagedIdentityCredential tokenCredential = new ManagedIdentityCredential();
                CosmosClient client = new CosmosClient("https://aiacosmosrbacdemo.documents.azure.com:443/", tokenCredential);
                Container container = client.GetContainer("eCommerceSampleDB", "UserReviews");
                ItemResponse<Review> res = await container.CreateItemAsync<Review>
                (
                    new Review() 
                    { 
                        Id = "c9e709b6-eb79-4d56-a1cc-cd98f4d4225a", 
                        username = "Sasha_Schaden56", 
                        product = "Socks",
                        review = "It only works when I'm asleep.",
                        rating = "1.2",
                        reviewDate = "2017-12-21T11:42:12.7505859+00:00",
                        documentType = "review"
                        
                    }, 
                    new PartitionKey("Socks")
                );
                return new OkObjectResult(JsonConvert.SerializeObject(res.Resource));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }
        }
    }
}
