Imports Microsoft.VisualBasic
Imports System

Module Program
    Sub Main()
        Console.WriteLine(vbCrLf + "What is your name? ")
        Dim name = Console.ReadLine()
        Dim currentDate = Now
        Console.WriteLine($"{vbCrLf}Hello, {name}, at {currentDate}!")
        Console.Write(vbCrLf + "Press any key to exit... ")
        Console.Read()
    End Sub
End Module
