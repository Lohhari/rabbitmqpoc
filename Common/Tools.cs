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
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public class Tools
    {
        public static byte[] MessageToBytes(Message message)
        {
            var bf = new BinaryFormatter();
            using(var ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                return ms.GetBuffer();
            }
        }

        public static Message BytesToMessage(byte[] bytes)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(bytes))
            {
                return (Message)bf.Deserialize(ms);
            }
        }

        public static QueueingBasicConsumer GetConsumer(IModel channel, string queueName)
        {
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.BasicQos(0, 1, false);
            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queueName, false, consumer);
            return consumer;
        }
    }
}
