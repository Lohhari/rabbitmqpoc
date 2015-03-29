/* RabbitMQPOC - A training exercise / proof of concept for Rabbit message queue usage
 * Written in 2015 by Vesa Pirila <vesa@pirila.fi> 
 *
 * To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights to this software to the public domain worldwide.
 * This software is distributed without any warranty. 
 * You should have received a copy of the CC0 Public Domain Dedication along with this software. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>. 
 */

using RabbitMQ.Client;
using System;
using System.IO;

namespace PushWork
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Give file path as argument");
                System.Environment.Exit(1);
            }

            var projectName = Path.GetFileNameWithoutExtension(args[0]);
            var projectId = System.Guid.NewGuid();
            Console.WriteLine("Processing " + projectName + " GUID: " + projectId);
            Common.FileStorage.CopyFileFrom(args[0], Common.Settings.InputFileName, projectId);

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(Common.Settings.SplitQueueName, true, false, false, null);

                    var message = new Common.Message(projectId, projectName);

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);

                    channel.BasicPublish("", Common.Settings.SplitQueueName, properties, Common.Tools.MessageToBytes(message));
                }
            }
        }
    }
}
