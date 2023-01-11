//#r "../../_/Xmly.Embed.dll"
//#load "data.cs"

using System;
using System.IO;
using System.Xml.Serialization;
using Xmly.Embed;
using XmlyC.Model;

namespace XmlyC
{
    internal static class Program
    {
        private static void Main()
        {
            var serializer = new XmlSerializer(typeof(Courses));
            var xml = Samples.Courses.Trim();

            using (var reader = new StringReader(xml))
            {
                var obj = (Courses)serializer.Deserialize(reader)!;

                foreach (var course in obj.CourseList)
                {
                    Console.WriteLine($"Course #{course.Id} '{course.Title}' " +
                                      $"costs {course.Price.Text} {course.Price.Currency}");
                }
            }
        }
    }
}
