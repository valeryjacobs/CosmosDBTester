using CosmosDBTesterModels;
using Microsoft.Azure.Documents.Client;
using System;
using System.Diagnostics;

namespace CosmosDBTestAgentConsole
{
    class Program
    {
        static DocumentClient _client;
        static void Main(string[] args)
        {
            var endPoint = Environment.GetEnvironmentVariable("COSMOSDB_Endpoint");
            var key = Environment.GetEnvironmentVariable("COSMOSDB_Key");

            _client = new DocumentClient(new Uri(endPoint), key);

            switch (args[0])
            {
                case "W":
                    WriteRandomDocuments(int.Parse(args[1]), args[2], args[3]);
                    break;
                case "R":
                    ReadDocuments(int.Parse(args[1]), args[2], args[3]);
                    break;
                default:
                    break;
            }

            Console.WriteLine("Waiting for re-run...");
            Console.Read();
        }

        private static async void ReadDocuments(int noItems, string databaseId, string collectionId)
        {
            Console.WriteLine("Reading " + noItems.ToString() + " documents...");

            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var docs = await _client.ReadDocumentFeedAsync(collectionLink, new FeedOptions { MaxItemCount = 100000 });

            foreach (var d in docs)
            {
                Console.WriteLine(d.Id);
            }

            stopWatch.Stop();

            Console.WriteLine("Finished reading " + noItems.ToString() + " documents in " + stopWatch.ElapsedMilliseconds + "ms.");
        }
        private static void WriteRandomDocuments(int noItems, string databaseId, string collectionId)
        {
            Stopwatch stopWatch = new Stopwatch();

            Console.WriteLine("Creating " + noItems.ToString() + " documents...");


            var categories = new[] { "apples", "bananas", "oranges", "grapes" };

            var productId = 0;
            var products = new Bogus.Faker<Product>()
                .StrictMode(true)
                .RuleFor(o => o.Id, f => productId++)
                .RuleFor(o => o.Category, f => f.PickRandom(categories))
                .RuleFor(o => o.Description, f => f.Lorem.Paragraph())
                .RuleFor(o => o.Image, f => f.Image.Food());

            var y = products.Generate(8);

            var orderItems = new Bogus.Faker<OrderItem>()
                 .StrictMode(true)
                 .RuleFor(o => o.Quantity, f => f.Random.Number(1, 1000))
                 .RuleFor(o => o.Product, f => products.Generate());

            var customerId = 0;
            var customers = new Bogus.Faker<Customer>()
                .StrictMode(true)
                .RuleFor(o => o.CustomerID, f => customerId++)
                .RuleFor(o => o.OrderTotal, f => f.Random.Double(0, 1000000))
                .RuleFor(o => o.Person, f => new Bogus.Person());

            var orderId = 0;
            var orders = new Bogus.Faker<Order>()
                 .StrictMode(true)
                 .RuleFor(o => o.Id, f => orderId++)
                 .RuleFor(o => o.Items, f => orderItems.Generate(10))
                 .RuleFor(o => o.DatePlaced, f => f.Date.Between(DateTime.Now.AddMonths(-15), DateTime.Now))
                 .RuleFor(o => o.Shipped, f => f.Random.Bool())
                 .RuleFor(o => o.ShippingState, f => f.Address.State())
                 .RuleFor(o => o.Customer, f => customers.Generate());

            var randomOrderDocuments = orders.Generate(noItems);

            Console.WriteLine();
            string[] indic = { "|", "/", "-", "\\" };
            int indecInd = 0;

            stopWatch.Start();
            foreach (Order order in randomOrderDocuments)
            {
                Console.Write("\b");
                Console.Write(indic[indecInd]);
                if (indecInd == 3) { indecInd = 0; } else { indecInd++; }

                _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), order);
            }

            stopWatch.Stop();

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Finished creating " + noItems.ToString() + " documents in " + stopWatch.ElapsedMilliseconds + "ms.");
        }
    }
}
