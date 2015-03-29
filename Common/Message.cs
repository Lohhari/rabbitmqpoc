/* RabbitMQPOC - A training exercise / proof of concept for Rabbit message queue usage
 * Written in 2015 by Vesa Pirila <vesa@pirila.fi> 
 *
 * To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights to this software to the public domain worldwide.
 * This software is distributed without any warranty. 
 * You should have received a copy of the CC0 Public Domain Dedication along with this software. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>. 
 */

using System;

namespace Common
{
    [Serializable]
    public class Message
    {
        public readonly System.Guid ProjectId;
        public string ProjectName;
        public int X;
        public int Y;
        public int Z;
        public int XSize;
        public int YSize;
        public int ZSize;

        public Message()
            : this(Guid.NewGuid(), string.Empty)
        { }

        public Message(System.Guid projectId, string projectName)
        {
            this.ProjectId = projectId;
            this.ProjectName = projectName;
        }

        public Message(Message other)
            : this(other.ProjectId, other.ProjectName)
        {
            this.XSize = other.XSize;
            this.YSize = other.YSize;
            this.ZSize = other.ZSize;
        }

        public string FileName
        {
            get { return string.Format("{0}-{1}-{2}", this.Z, this.Y, this.X); }
        }

        public int Size
        {
            get { return XSize * YSize * ZSize; }
        }
    }
}
