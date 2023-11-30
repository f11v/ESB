using ESB;
using System;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Elija la operación: [P]roducir mensaje, [C]onsumir mensajes, [S]alida");
            var operation = Console.ReadLine()?.ToLower();

            try
            {
                switch (operation)
                {
                    case "p":
                        var producer = new MessageProducer();
                        producer.Run();
                        break;

                    case "c":
                        var consumer = new MessageConsumer();
                        consumer.Run();
                        break;

                    case "s":
                        Console.WriteLine("Saliendo del programa...");
                        return;

                    default:
                        Console.WriteLine("Operación inválida. Por favor, escriba 'P', 'C' o 'S'.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
