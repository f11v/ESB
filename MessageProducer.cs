using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESB
{
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
}
