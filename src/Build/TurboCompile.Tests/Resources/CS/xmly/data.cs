using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlyC.Model
{
    [XmlRoot("Courses")]
    public class Courses
    {
        [XmlElement(ElementName = "Academy")] 
        public string Academy { get; set; }

        [XmlElement(ElementName = "Course")] 
        public List<Course> CourseList { get; set; }

        public Courses()
        {
            CourseList = new List<Course>();
        }
    }

    [XmlRoot(ElementName = "Course")]
    public class Course
    {
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Duration")]
        public string Duration { get; set; }

        [XmlElement(ElementName = "Instructor")]
        public string Instructor { get; set; }

        [XmlElement(ElementName = "Price")]
        public Price Price { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlText]
        public string Text { get; set; }

        public Course()
        {
            Price = new Price();
        }
    }

    [XmlRoot(ElementName = "Price")]
    public class Price
    {
        [XmlAttribute(AttributeName = "Currency")]
        public string Currency { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}