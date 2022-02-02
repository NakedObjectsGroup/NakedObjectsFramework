Imports NakedFramework

Imports System.Collections.Generic



Public Class Teacher
    Implements IContainerAware
    Public Property Container As IContainer Implements IContainerAware.Container

    Public Property Id As Integer

    Public Property mappedFullName As String
    Private myFullName As TextString

    <DemoProperty(Order:=1)>
    Public ReadOnly Property FullName As TextString
        Get
            myFullName = If(myFullName, New TextString(mappedFullName, Sub(v) mappedFullName = v))
            Return myFullName
        End Get
    End Property

    <TableView(False, "Subject", "YearGroup", "SetName")>
    Public Function ActionSetsTaught() As IQueryable(Of TeachingSet)
        Return Container.AllInstances(Of TeachingSet)().Where(Function(s) s.Teacher.Id = Id).OrderBy(Function(s) s.Subject.mappedName).ThenBy(Function(s) s.mappedYearGroup)
    End Function

    Public Sub ActionSave()
        Dim m = mappedFullName
        Container.MakePersistent(Me)
    End Sub

    Public Overrides Function ToString() As String
        Return FullName.Value
    End Function

    Public Function Title() As Title
        Return New Title(ToString())
    End Function
End Class

