using Couchbase.Lite;
using Couchbase.Lite.Sync;

namespace SyncTestiConsole
{
    internal class Program
    {
        static Task Main(string[] args)
        {
            Collection airlineCollection, spedificusrtestCollection;
            

            var database = new Database("mydb");
            airlineCollection = database.CreateCollection("airline", "inventory");
            spedificusrtestCollection = database.CreateCollection("specifcusrtest", "inventory");
            


            // Define the Sync Gateway URL
            var gatewayUrl = new Uri("wss://xxx.apps.cloud.couchbase.com:4984/chanelsendppoint");

            // Create the replicator configuration using the new constructor
            var replicatorConfiguration = new ReplicatorConfiguration(new URLEndpoint(gatewayUrl))
            {
                ReplicatorType = ReplicatorType.PushAndPull,
                Continuous = false,
                Authenticator = new BasicAuthenticator("usrname", "pwd")
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
