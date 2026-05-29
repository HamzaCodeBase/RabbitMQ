using System.Text;
using RabbitMQ.Client;

// 1. Create a connection factory pointing to your local Docker instance
var factory = new ConnectionFactory { HostName = "localhost" };

// 2. Open the connection and create a channel
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// 3. Declare a queue so we know it exists before publishing
// (QueueName, Durable, Exclusive, AutoDelete, Arguments)
string queueName = "learning_queue";
await channel.QueueDeclareAsync(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

for(int i = 1; i <= 5; i++)
{
    string message = $"Hello, this is message #{i}!";
    var body = Encoding.UTF8.GetBytes(message);
    // 4. Publish the message to the default exchange (empty string)
    // The default exchange automatically routes messages to the queue matching the routingKey
    await channel.BasicPublishAsync(exchange: string.Empty,
                                     routingKey: queueName,
                                     body: body);
    Console.WriteLine($"[x] Sent: '{message}'");
    await Task.Delay(3000);
}
