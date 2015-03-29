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

namespace Combine
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
                    Console.WriteLine("Waiting for combine messages. To exit press CTRL+C");

                    var combineConsumer = Common.Tools.GetConsumer(channel, Common.Settings.CombineQueueName);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)combineConsumer.Queue.Dequeue();
                        var message = (Common.Message)Common.Tools.BytesToMessage(ea.Body);

                        int sum = CombineInput(message);
                        Console.WriteLine("Final result for Guid: {0} Project name: {1} is {2}", message.ProjectId, message.ProjectName, sum);
                        Common.FileStorage.WriteFile(message.ProjectId, Common.Settings.OutputFileName, sum.ToString());

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        private static int CombineInput(Common.Message message)
        {
            try
            {
                int sum = 0;

                for (int z = 0; z < message.ZSize; z++)
                {
                    for (int y = 0; y < message.YSize; y++)
                    {
                        for (int x = 0; x < message.XSize; x++)
                        {
                            var tempMessage = new Common.Message() { X = x, Y = y, Z = z };
                            var lines = Common.FileStorage.ReadAllLines(
                                message.ProjectId, tempMessage.FileName + Common.Settings.OutputSuffix);

                            sum += Convert.ToInt32(lines[0]);
                        }
                    }
                }

                return sum;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Parse error: index out of range");
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                Console.WriteLine("Parse error: directory not found\n" + ex.Message);
            }

            throw new Exception("Combining results failed");
        }
    }
}
