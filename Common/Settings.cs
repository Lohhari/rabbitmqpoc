/* RabbitMQPOC - A training exercise / proof of concept for Rabbit message queue usage
 * Written in 2015 by Vesa Pirila <vesa@pirila.fi> 
 *
 * To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights to this software to the public domain worldwide.
 * This software is distributed without any warranty. 
 * You should have received a copy of the CC0 Public Domain Dedication along with this software. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>. 
 */

namespace Common
{
    public class Settings
    {
        public const string SplitQueueName = "split";
        public const string ProcessQueueName = "process";
        public const string CollectQueueName = "collect";
        public const string CombineQueueName = "combine";

        public const string InputFileName = "input.txt";
        public const string InputSuffix = ".in.txt";
        public const string OutputSuffix = ".out.txt";
    }
}
