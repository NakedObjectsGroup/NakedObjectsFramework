Imports NakedFramework

Imports System.Collections.Generic



Public Class Teacher
    Implements IContainerAware
    Public Property Container As IContainer Implements IContainerAware.Container

    Public Property Id As Integer

    Public mappedFullName As String
    Private myFullName As TextString

    <DemoProperty(Order:=1)>
    Public ReadOnly Property FullName As TextString
        Get
            myFullName = If(myFullName, New TextString(mappedFullName, Function(v) mappedFullName = v))
            Return myFullName
        End Get
    End Property

    'TODO <TableView(False, "Subject", "YearGroup", "SetName")>
    Public Function ActionSetsTaught() As IQueryable(Of TeachingSet)
        Return Container.AllInstances(Of TeachingSet)().Where(Function(s) s.Teacher.Id = Id).OrderBy(Function(s) s.Subject.mappedName).ThenBy(Function(s) s.mappedYearGroup)
    End Function

    Public Overrides Function ToString() As String
        Return FullName.Value
    End Function

    Public Function Title() As Title
        Return New Title(ToString())
    End Function
End Class

