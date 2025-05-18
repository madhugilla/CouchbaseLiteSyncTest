using Couchbase.Lite;
using Couchbase.Lite.Sync;
using Microsoft.Extensions.Configuration;

namespace SyncTestiConsole
{
    internal class Program
    {
        static Task Main(string[] args)
        {
            // Set up configuration to read from user secrets
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            
            // Read secrets
            var gatewayUrlString = configuration["CouchbaseSettings:GatewayUrl"];
            var username = configuration["CouchbaseSettings:Username"];
            var password = configuration["CouchbaseSettings:Password"];
            
            if (string.IsNullOrEmpty(gatewayUrlString) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Error: Connection settings not found in secrets. Please set up user secrets with the required values.");
                return Task.CompletedTask;
            }

            Collection airlineCollection, spedificusrtestCollection;

            var database = new Database("mydb");
            airlineCollection = database.CreateCollection("airline", "inventory");
            spedificusrtestCollection = database.CreateCollection("specifcusrtest", "inventory");

            // Define the Sync Gateway URL
            var gatewayUrl = new Uri(gatewayUrlString);

            // Create the replicator configuration using the new constructor
            var replicatorConfiguration = new ReplicatorConfiguration(new URLEndpoint(gatewayUrl))
            {
                ReplicatorType = ReplicatorType.PushAndPull,
                Continuous = false,
                Authenticator = new BasicAuthenticator(username, password)
            };

            string currentUserId = "mosel2";
            // Set the collection configuration with channels
            var collectionConfig = new CollectionConfiguration
            {
                //Channels = new[] { "airline" },
                Channels = new[] { $"user::{currentUserId}" }
            };

            // replicatorConfiguration.AddCollection(airlineCollection, collectionConfig);
            replicatorConfiguration.AddCollection(spedificusrtestCollection, collectionConfig);

            // Initialize the replicator
            var replicator = new Replicator(replicatorConfiguration);

            // Add a change listener to monitor replication status
            replicator.AddChangeListener((sender, args) =>
            {
                var status = args.Status;
                Console.WriteLine($"Replication Status: {status.Activity}, " +
                                $"Completed: {status.Progress.Completed}, " +
                                $"Total: {status.Progress.Total}, " +
                                $"Error: {status.Error?.Message ?? "None"}");
            });

            // Start the replication
            replicator.Start();
            Console.WriteLine("Replication started...");

            // Wait for user input to stop the replication
            Console.WriteLine("Press any key to stop the replication...");
            Console.ReadKey();
            return Task.CompletedTask;
        }
    }
}
