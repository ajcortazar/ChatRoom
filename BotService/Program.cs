using System;

namespace BotService
{
    class Program
    {
        private static string _queueName = "stock_queue";

        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for messages.");
            Service server = new Service(_queueName);
            var connection = server.GetConnection();
            var model = connection.CreateModel();
            server.ReceiveRpcMessage(model);
            Console.ReadLine();
        }
    }
}
