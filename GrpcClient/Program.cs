using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcService;

namespace GrpcClient
{
    public class Program
    {
        private static Random _random;

        public static async Task Main(string[] args)
        {
            _random = new Random();

            // - Simple Unary API
            var channel = GrpcChannel.ForAddress("http://localhost:5145");
            var client = new Sample.SampleClient(channel);
            var response = await client.GetFullNameAsync(new SampleRequest { FirstName = "John", LastName = "Thomas" });
            Console.WriteLine(response.FullName);

            // - Passing a complex object in Request & Response
            var client2 = new Product.ProductClient(channel);
            var stockDate = DateTime.SpecifyKind(new DateTime(2022, 2, 1), DateTimeKind.Utc);
            var response2 = await client2.SaveProductAsync(new ProductModel
            {
                ProductName = "Macbook Pro",
                ProductCode = "P1001",
                Price = 5000,
                StockDate = Timestamp.FromDateTime(stockDate)
            });
            Console.WriteLine($"{response2.StatusCode} | {response2.IsSuccessful}");

            // - Passing a list of obbjects in Response
            var response3 = await client2.GetProductsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (var product in response3.Products)
            {
                var stockDate1 = product.StockDate.ToDateTime();
                Console.WriteLine($"{product.ProductName} | {product.ProductCode} | {product.Price} | {stockDate1.ToString("dd-MM-yyyy")}");
            }


            // - Closing the Channel after completion
            Console.WriteLine("Closing the Channel");
            await channel.ShutdownAsync();


            Console.ReadKey();
        }
    }
}
