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
using System.Threading;

namespace Split
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
                    Console.WriteLine("Waiting for split messages. To exit press CTRL+C");

                    var splitConsumer = Common.Tools.GetConsumer(channel, Common.Settings.SplitQueueName);
                    channel.QueueDeclare(Common.Settings.ProcessQueueName, true, false, false, null);
                                        var processProperties = channel.CreateBasicProperties();
                    processProperties.SetPersistent(true);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)splitConsumer.Queue.Dequeue();
                        var message = (Common.Message)Common.Tools.BytesToMessage(ea.Body);
                        Console.WriteLine("Received Guid: {0} Project name: {1}", message.ProjectId, message.ProjectName);

                        var processMessages = SplitInput(message);

                        Console.WriteLine("Split done");

                        foreach(var processMessage in processMessages)
                        {
                            channel.BasicPublish("", Common.Settings.ProcessQueueName, processProperties, Common.Tools.MessageToBytes(processMessage));
                        }

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        private static List<Common.Message> SplitInput(Common.Message message)
        {
            try
            {
                var lines = Common.FileStorage.ReadAllLines(message.ProjectId, Common.Settings.InputFileName);

                var newMessages = new List<Common.Message>();
                var dimensions = lines[0].Split(' ');
                message.XSize = int.Parse(dimensions[0]);
                message.YSize = int.Parse(dimensions[1]);
                message.ZSize = int.Parse(dimensions[2]);

                int i = 1;

                for (int z = 0; z < message.ZSize; z++)
                {
                    for (int y = 0; y < message.YSize; y++)
                    {
                        for (int x = 0; x < message.XSize; x++)
                        {
                            var newMessage = new Common.Message(message) { X = x, Y = y, Z = z };
                            Common.FileStorage.WriteFile(newMessage.ProjectId, newMessage.FileName + Common.Settings.InputSuffix, lines[i]);
                            newMessages.Add(newMessage);
                            i++;
                        }
                    }
                }

                return newMessages;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Parse error: index out of range");
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                Console.WriteLine("Parse error: directory not found\n" + ex.Message);
            }

            return null;
        }
    }
}
