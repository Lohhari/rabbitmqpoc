/* RabbitMQPOC - A training exercise / proof of concept for Rabbit message queue usage
 * Written in 2015 by Vesa Pirila <vesa@pirila.fi> 
 *
 * To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights to this software to the public domain worldwide.
 * This software is distributed without any warranty. 
 * You should have received a copy of the CC0 Public Domain Dedication along with this software. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>. 
 */

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace Collect
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var counts = new Dictionary<System.Guid, int>();
                    Console.WriteLine("Waiting for collect messages. To exit press CTRL+C");

                    var collectConsumer = Common.Tools.GetConsumer(channel, Common.Settings.CollectQueueName);

                    channel.QueueDeclare(Common.Settings.CombineQueueName, true, false, false, null);
                    var combineProperties = channel.CreateBasicProperties();
                    combineProperties.SetPersistent(true);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)collectConsumer.Queue.Dequeue();
                        var message = (Common.Message)Common.Tools.BytesToMessage(ea.Body);

                        if (!counts.ContainsKey(message.ProjectId))
                        {
                            counts[message.ProjectId] = 1;
                        }
                        else
                        {
                            counts[message.ProjectId]++;
                        }

                        if (counts[message.ProjectId] == message.Size)
                        {
                            Console.WriteLine("Received Guid: {0} Project name: {1} Count: {2}", message.ProjectId, message.ProjectName, counts[message.ProjectId]);
                            Console.WriteLine("All segments processed in project " + message.ProjectId);
                            // X, Y and Z parameters do not matter in the Combine step. We can re-use the message that happened to arrive last.
                            channel.BasicPublish("", Common.Settings.CombineQueueName, combineProperties, Common.Tools.MessageToBytes(message));
                        }
                        else
                        {
                            Console.WriteLine("Received Guid: {0} Project name: {1} Count: {2}", message.ProjectId, message.ProjectName, counts[message.ProjectId]);
                        }

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }
    }
}
