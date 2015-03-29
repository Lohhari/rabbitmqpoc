/* RabbitMQPOC - A training exercise / proof of concept for Rabbit message queue usage
 * Written in 2015 by Vesa Pirila <vesa@pirila.fi> 
 *
 * To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights to this software to the public domain worldwide.
 * This software is distributed without any warranty. 
 * You should have received a copy of the CC0 Public Domain Dedication along with this software. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>. 
 */

using System;
using System.IO;

namespace Common
{
    public class FileStorage
    {
        const string RootPath = @"d:\t\";

        public static void CopyFileFrom(string fromPath, string toFileName, System.Guid projectId)
        {
            Directory.CreateDirectory(GetFolderPath(projectId));
            File.Copy(fromPath, Path.Combine(RootPath, projectId.ToString(), toFileName));
        }

        public static string[] ReadAllLines(System.Guid projectId, string fileName)
        {
            return File.ReadAllLines(Path.Combine(GetFolderPath(projectId), fileName));
        }

        public static void WriteFile(System.Guid projectId, string fileName, string content)
        {
            File.WriteAllText(Path.Combine(GetFolderPath(projectId), fileName), content);
        }

        private static string GetFolderPath(System.Guid projectId)
        {
            return Path.Combine(RootPath, projectId.ToString());
        }
    }
}
