# Couchbase Lite Sync Test Console Application

## Overview

This console application demonstrates how to use Couchbase Lite with .NET to synchronize data with a remote Couchbase Sync Gateway. It specifically showcases:

- Creating and managing collections in Couchbase Lite
- Setting up replication with a Sync Gateway
- Using channel-based sync with user-specific channels
- Monitoring replication status

## Prerequisites

- .NET 8.0 SDK or later
- Couchbase Lite for .NET package
- Access to a Couchbase Sync Gateway instance

## Project Structure

The project consists of a simple console application with the following structure:

- `Program.cs` - Main application code that sets up the database, collections, and replication
- `SyncTestiConsole.csproj` - Project file containing NuGet references
- `bin/Debug/net8.0/CouchbaseLite/mydb.cblite2/` - Local database storage location

## NuGet Packages

This project uses the following NuGet packages:

- `Couchbase.Lite` - Core Couchbase Lite functionality
- `Couchbase.Lite.Support.NetDesktop` - Desktop support for Couchbase Lite

## How It Works

1. **Database and Collection Setup**:
   The application creates a local database named "mydb" and initializes two collections:
   - `airline` in the `inventory` scope
   - `specifcusrtest` in the `inventory` scope

2. **Replication Configuration**:
   - Configures bidirectional replication (push and pull) with the Sync Gateway
   - Uses Basic Authentication for accessing the Sync Gateway
   - Configures user-specific channels for data access control

3. **Replication Monitoring**:
   - Sets up event listeners to monitor replication status
   - Displays real-time progress updates in the console

4. **Execution Flow**:
   - Initialize database and collections
   - Configure and start replication
   - Monitor replication status through console output
   - Wait for user input to stop the replication

## Usage

1. Update the Sync Gateway URL in the code to point to your Couchbase Sync Gateway instance:
   ```csharp
   var gatewayUrl = new Uri("wss://your-gateway-url:4984/your-database");
   ```

2. Update the authentication credentials:
   ```csharp
   Authenticator = new BasicAuthenticator("username", "password")
   ```

3. Modify the channel configuration based on your Sync Gateway setup:
   ```csharp
   Channels = new[] { $"user::{currentUserId}" }
   ```

4. Run the application and observe the replication status in the console.

5. Press any key to stop replication and exit.

## Customization

To adapt this code for different use cases:

- **Different Collection Structure**: Modify the collection names and scopes during creation
- **Replication Type**: Change the `ReplicatorType` to `Pull` or `Push` if you only need one-way replication
- **Continuous Replication**: Set `Continuous = true` for ongoing replication instead of a one-time sync
- **Different Channel Strategy**: Update the `Channels` array in the collection configuration

## Couchbase Sync Gateway Configuration

### Access Control and Data Validation

To enable the user-specific channel access in Couchbase, the following JavaScript function needs to be configured on the collection's "Access Control and Data Validation" settings in Couchbase:

```javascript
function (doc, oldDoc, meta) {
  channel("user::" + doc.userId);
}
```

This function routes each document to a user-specific channel based on the document's `userId` field. This ensures that:

1. Documents are only accessible to users who have access to that specific channel
2. The sync process filters documents based on the user's channel access
3. Data remains properly segmented per user

### How to Configure in Couchbase

1. Log in to the Couchbase Server Admin Console
2. Navigate to the "Collections" section for your bucket
3. Select the appropriate scope and collection (in our case, "inventory.specifcusrtest")
4. Click on "Access Control and Data Validation" settings
5. Paste the JavaScript function into the editor
6. Save the configuration

## Troubleshooting

Common issues:

1. **Authentication Failure**: Check that your username and password match those configured on the Sync Gateway
2. **Connection Issues**: Verify the Sync Gateway URL is correct and accessible
3. **Channel Access**: Ensure the specified channels exist and the authenticated user has access to them
4. **Sync Function Errors**: Verify that the sync function is correctly configured and does not contain syntax errors

## License

This project is provided as a sample and is not covered by any specific license.
