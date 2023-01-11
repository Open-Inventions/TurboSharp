Imports System.Collections.Generic
Imports System.Xml.Serialization

<XmlRoot("Courses")>
Public Class Courses
    <XmlElement(ElementName:="Academy")>
    Public Property Academy As String

    <XmlElement(ElementName:="Course")>
    Public Property CourseList As List(Of Course)

    Public Sub New()
        CourseList = New List(Of Course)()
    End Sub
End Class

<XmlRoot(ElementName:="Course")>
Public Class Course
    <XmlElement(ElementName:="Title")>
    Public Property Title As String

    <XmlElement(ElementName:="Duration")>
    Public Property Duration As String

    <XmlElement(ElementName:="Instructor")>
    Public Property Instructor As String

    <XmlElement(ElementName:="Price")>
    Public Property Price As Price

    <XmlAttribute(AttributeName:="Id")>
    Public Property Id As Integer

    <XmlText>
    Public Property Text As String

    Public Sub New()
        Price = New Price()
    End Sub
End Class

<XmlRoot(ElementName:="Price")>
Public Class Price
    <XmlAttribute(AttributeName:="Currency")>
    Public Property Currency As String

    <XmlText>
    Public Property Text As String
End Class
