using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CosmosDBTestAgent.Controllers
{
    [Route("api/[controller]")]
    public class DocumentsController : Controller
    {
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var endPoint = Environment.GetEnvironmentVariable("COSMOSDB_Endpoint");
            var key = Environment.GetEnvironmentVariable("COSMOSDB_Key");

            DocumentClient client = new DocumentClient(new Uri(endPoint), key);

            Generate();

            return new string[] { "value1", "value2" };
        }

        private static async Task Generate()
        {



        }
    }
}
