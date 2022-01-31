Imports NakedFramework

Imports System.Collections.Generic



Public Class Teacher
        Public Property Container As IContainer

    Public Overridable Property Id As Integer
    <DemoProperty(Order:=1)>
    Public Overridable Property FullName As String

    <DemoProperty(Order:=5)>
    <TableView(False, "Subject", "YearGroup", "SetName")>
    Public Overridable ReadOnly Property SetsTaught() As ICollection(Of [Set])
        Get
            Dim id As Integer = Me.Id
            Return Container.AllInstances(Of [Set])().Where(Function(s) s.Teacher.Id = id).OrderBy(Function(s) s.Subject.Name).ThenBy(Function(s) s.YearGroup).ToList()
        End Get
    End Property

    Public Overrides Function ToString() As String
            Return FullName
        End Function
    End Class

