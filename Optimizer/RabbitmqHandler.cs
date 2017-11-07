using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    class RabbitmqHandler
    {
        //public void Publish(model.Parameters parameters)
        public void Publish(model.ShortParameters parameters, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declaring a queue is idempotent - it will only be created if it doesn't exist already
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var properties = channel.CreateBasicProperties();
                // this will mark the msg persistent by writing it to cache and disk
                properties.Persistent = true;

                // create the message; msg is a byte array, encode whatever u like
                var message = parameters.ToString();
                var body = Encoding.UTF8.GetBytes(message);

                // publish is the key action for producer; send to the queue
                // the task will only send once. once the task is sent to the queue, it will out of the channel.
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine("Published a parameter set to worker queue --- " + message);
            }

        }

        /// <summary>
        /// Consumes the parameters. the worker mether to get one task
        /// </summary>
        public void Consume(string queueName, int maxNum, string logFilePath)
        {
            var messageNum = 0;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var file = new StreamWriter(logFilePath, true))
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine("Waiting for log messages...");

                while (messageNum < maxNum)
                {
                    var message = channel.BasicGet(queue: queueName, autoAck: false);
                    if (message == null)
                    {
                        //throw new Exception("Error: Empty log message received, Exit!");
                        //channel.BasicAck(deliveryTag: message.DeliveryTag, multiple: false);
                        continue;
                    }

                    // get the msg
                    var body = message.Body;
                    var msg = Encoding.UTF8.GetString(body);

                    // store the msg
                    file.WriteLine(msg);
                    Console.WriteLine("Received a log message.");

                    // this line is important. rabbitMQ will not realease unless it's acked
                    channel.BasicAck(deliveryTag: message.DeliveryTag, multiple: false);

                    messageNum++;
                }
                
            }
        }
    }
}
