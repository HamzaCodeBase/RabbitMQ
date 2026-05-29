using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

string queueName = "learning_queue";

// Declare the queue here too, in case the consumer starts up first
await channel.QueueDeclareAsync(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

Console.WriteLine("[*] Waiting for messages. Press Ctrl+C to exit.");

// 1. Create an asynchronous consumer
var consumer = new AsyncEventingBasicConsumer(channel);

// 2. Define what happens when a message arrives
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Received: '{message}'");

    // Simulate some work
    await Task.CompletedTask;
};

// 3. Start consuming from the queue
await channel.BasicConsumeAsync(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

// Keep the console app running so it can listen for incoming messages
await Task.Delay(Timeout.Infinite);