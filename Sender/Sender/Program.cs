using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                var queueName = "multiple-channels-and-workers";

                for (var i = 0; i <= 5; i++)
                {
                    var channel = CreateChannel(connection);

                    BuildPublishers(channel, queueName, $"canal {i}");
                }

                Console.ReadLine();
            }
        }

        public static IModel CreateChannel(IConnection connection) =>
            connection.CreateModel();

        public static void BuildPublishers(IModel channel, string queue, string publisherName)
        {
            Task.Run(() =>
            {
                var count = 0;

                channel.QueueDeclare(
                    queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                while (true)
                {
                    string message = $"Menssagem de número {count++}, do {publisherName}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", queue, null, body);

                    Console.WriteLine("Enviado {0}", message);
                    System.Threading.Thread.Sleep(1000);
                }
            });
        }
    }
}
