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
using System.Threading;

namespace Process
{
    class Program
    {
        const int Delay = 400;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("Waiting for process messages. To exit press CTRL+C");

                    var processConsumer = Common.Tools.GetConsumer(channel, Common.Settings.ProcessQueueName);
                    
                    channel.QueueDeclare(Common.Settings.CollectQueueName, true, false, false, null);
                    var collectProperties = channel.CreateBasicProperties();
                    collectProperties.SetPersistent(true);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)processConsumer.Queue.Dequeue();
                        var message = (Common.Message)Common.Tools.BytesToMessage(ea.Body);
                        Console.WriteLine("Received Guid: {0} Project name: {1} X: {2} Y: {3} Z: {4}",
                            message.ProjectId, message.ProjectName, message.X, message.Y, message.Z);

                        ProcessInput(message);
                        channel.BasicPublish("", Common.Settings.CollectQueueName, collectProperties, Common.Tools.MessageToBytes(message));

                        Console.WriteLine("Processing complete");

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        private static void ProcessInput(Common.Message message)
        {
            try
            {
                var lines = Common.FileStorage.ReadAllLines(message.ProjectId, message.FileName + Common.Settings.InputSuffix);
                int value = Convert.ToInt32(lines[0]);
                Thread.Sleep(Delay);
                Common.FileStorage.WriteFile(message.ProjectId, message.FileName + Common.Settings.OutputSuffix, (value * 2).ToString());
            }
            catch (System.FormatException ex)
            {
                Console.WriteLine("Input was not an integer\n" + ex.Message);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                Console.WriteLine("Parse error: directory not found\n" + ex.Message);
            }
        }
    }
}
