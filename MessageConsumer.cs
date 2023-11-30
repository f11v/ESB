using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESB
{
    class MessageConsumer
    {
        private int messageCount = 0;

        public void Run()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "messages", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    messageCount++;
                    Console.WriteLine($"{messageCount}. Recibió '{message}'");
                };

                channel.BasicConsume(queue: "messages", autoAck: false, consumer: consumer);

                Console.WriteLine("Ingrese el número del mensaje a eliminar (presione [enter] para salir):");

                while (true)
                {
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        break;
                    }

                    if (int.TryParse(input, out int messageToDelete) && messageToDelete >= 1 && messageToDelete <= messageCount)
                    {
                        channel.BasicAck(deliveryTag: (ulong)messageToDelete, multiple: false);
                        Console.WriteLine($"Mensaje eliminado {messageToDelete}");
                    }
                    else
                    {
                        Console.WriteLine("Entrada inválida. Ingrese un número de mensaje válido o presione [enter] para salir.");
                    }
                }
            }
        }
    }
}
