using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Challenge.Chatroom.Models
{
    public class ServiceClient
    {
        private string _queueName;
        private string _responseQueue;
        private QueueingBasicConsumer _consumer;

        public ServiceClient(string queue)
        {
            _queueName = queue;
        }

        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            return factory.CreateConnection();
        }

        public void SetUpQueue(IModel model)
        {
            model.QueueDeclare(_queueName, true, false, false, null);
        }

        public string SendMessageToQueue(string message, IModel model, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(_responseQueue))
            {
                _responseQueue = model.QueueDeclare().QueueName;
            }

            if (_consumer == null)
            {
                _consumer = new QueueingBasicConsumer(model);
                model.BasicConsume(_responseQueue, true, _consumer);
            }

            string correlationId = Guid.NewGuid().ToString();

            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.ReplyTo = _responseQueue;
            basicProperties.CorrelationId = correlationId;

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            model.BasicPublish("", _queueName, basicProperties, messageBytes);

            DateTime timeoutDate = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow <= timeoutDate)
            {
                BasicDeliverEventArgs deliveryArguments = _consumer.Queue.Dequeue();
                if (deliveryArguments.BasicProperties != null && deliveryArguments.BasicProperties.CorrelationId == correlationId)
                {
                    string response = Encoding.UTF8.GetString(deliveryArguments.Body);
                    return response;
                }
            }
            throw new TimeoutException("No response before the timeout period.");
        }
    }
}
