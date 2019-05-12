using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace BotService
{
    public class Service
    {
        private string _queueName;        
        private string _responseQueue;
        private QueueingBasicConsumer _consumer;

        public Service(string queue)
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

        public void ReceiveRpcMessage(IModel model)
        {
            try
            {
                model.BasicQos(0, 1, false);
                QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
                model.BasicConsume(_queueName, false, consumer);

                while (true)
                {
                    BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                    string message = Encoding.UTF8.GetString(deliveryArguments.Body);
                    string response = ManageFile(message);
                    IBasicProperties replyBasicProperties = model.CreateBasicProperties();
                    replyBasicProperties.CorrelationId = deliveryArguments.BasicProperties.CorrelationId;
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    model.BasicPublish("", deliveryArguments.BasicProperties.ReplyTo, replyBasicProperties, responseBytes);
                    model.BasicAck(deliveryArguments.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public static string ManageFile(string quote)
        {
            string url = string.Format("https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv", quote.ToLower());
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            result = responseStream.ReadToEnd();
            var msg = GetQuoteInformationFromCsv(quote, result);
            webresponse.Close();
            return msg;
        }

        public static string GetQuoteInformationFromCsv(string quote, string csvContent)
        {
            var value = csvContent.Split(',')[13];
            var result = string.Empty;
            if(value != "N/D")
            {
                result = string.Format("{0} quote is ${1} per share", quote, value);
            }
            else
            {
                result = string.Format("quote {0} doesn't have a value.", quote);
            }
            
            return result;
        }
    }
}
