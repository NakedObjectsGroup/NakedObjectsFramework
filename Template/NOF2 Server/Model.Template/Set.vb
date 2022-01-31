Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Imports NakedFramework



Public Class [Set]
    Public Property Container As IContainer

    Public Overridable Property Id As Integer

    <DemoProperty(Order:=1)>
    Public Overridable Property SetName As String
    <DemoProperty(Order:=2)>
    Public Overridable Property Subject As Subject
    <DemoProperty(Order:=3), Range(9, 13)>
    Public Overridable Property YearGroup As Integer
    <DemoProperty(Order:=4)>
    Public Overridable Property Teacher As Teacher

    <TableView(False, "FullName")>
    <DemoProperty(Order:=5)>
    Public Overridable Property Students As ICollection(Of Student) = New List(Of Student)()

    Public Sub AddStudentToSet(ByVal student As Student)
        Students.Add(student)
    End Sub

    Public Function RemoveStudentFromSet(ByVal student As Student)
        Return Students.Remove(student)
    End Function

    Public Function Choices0RemoveStudentFromSet() As IList(Of Student)
        Return Students.ToList()
    End Function

    Public Sub TransferStudentTo(ByVal student As Student, ByVal newSet As [Set])
        RemoveStudentFromSet(student)
        newSet.AddStudentToSet(student)
    End Sub

    Public Function Choices0TransferStudentTo() As IList(Of Student)
        Return Students.ToList()
    End Function


    Public Function Choices1TransferStudentTo() As IList(Of [Set])
        Dim subjId As Integer = Subject.Id
        Dim yg As Integer = YearGroup
        Return Container.AllInstances(Of [Set])().Where(Function(s) s.Subject.Id = subjId AndAlso s.YearGroup = yg).ToList()
    End Function

    Public Overrides Function ToString() As String
        Return SetName
    End Function
End Class

