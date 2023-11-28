using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Elija la operación: [P]roducir mensaje, [C]onsumir mensajes, [S]alida");
            var operation = Console.ReadLine();

            if (operation.ToLower() == "p")
            {
                var producer = new MessageProducer();
                producer.Run();
            }
            else if (operation.ToLower() == "c")
            {
                var consumer = new MessageConsumer();
                consumer.Run();
            }
            else if (operation.ToLower() == "s")
            {
                break;
            }
            else
            {
                Console.WriteLine("Operación inválida. Por favor, escriba 'P', 'C' o 'S'.");
            }
        }

        Console.WriteLine("Saliendo del programa...");
    }
}

class MessageProducer
{
    public void Run()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "messages", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("Ingresa el mensaje:");
            string messageText = Console.ReadLine();

            // Agregar detalles al mensaje, como la hora de envío y el remitente (IP DEL EQUIPO).
            string fullMessage = $"{DateTime.Now} - From: {GetLocalIPAddress()} - {messageText}";

            var body = Encoding.UTF8.GetBytes(fullMessage);
            channel.BasicPublish(exchange: "", routingKey: "messages", basicProperties: null, body: body);

            Console.WriteLine($" Mensaje enviado: '{fullMessage}'");
        }
    }

    // Método para obtener la dirección IP local del equipo.
    private string GetLocalIPAddress()
    {
        string localIp = "";
        System.Net.IPHostEntry host;
        host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (System.Net.IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIp = ip.ToString();
                break;
            }
        }
        return localIp;
    }
}

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



