'#r "../../_/Xmly.Embed.dll"
'#load "data.vb"

Imports System
Imports System.IO
Imports System.Xml.Serialization
Imports Xmly.Embed
Imports XmlyV.Model

Module Program
    Sub Main()
        Dim serializer = New XmlSerializer(GetType(Courses))
        Dim xml = Samples.Courses.Trim()

        Using reader = New StringReader(xml)
            Dim obj = CType(serializer.Deserialize(reader), Courses)

            For Each course In obj.CourseList
                Console.WriteLine($"Course #{course.Id} '{course.Title}' " &
                                  $"costs {course.Price.Text} {course.Price.Currency}")
            Next
        End Using
    End Sub
End Module
