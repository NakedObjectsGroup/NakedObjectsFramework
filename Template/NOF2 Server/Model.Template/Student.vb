
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations

Imports NakedFramework


    Public Class Student
        Public Property Teachers As TeacherRepository
        Public Property Container As IContainer

    Public Overridable Property Id As Integer
    <DemoProperty(Order:=1)>
    Public Overridable Property FullName As String
    <DemoProperty(Order:=2), Range(9, 13)>
    Public Overridable Property CurrentYearGroup As Integer
    <DemoProperty(Order:=4)>
    Public Overridable Property PersonalTutor As Teacher

    Public Function ChoicesPersonalTutor() As IList(Of Teacher)
        Return Teachers.AllTeachers().ToList()
    End Function

    <DemoProperty(Order:=3)>
    <TableView(False, "Subject", "SetName", "Teacher")>
        Public Overridable Property Sets As ICollection(Of [Set]) = New List(Of [Set])()

    Public Function ListRecentReports(
 ByVal forSubject As Subject) As IQueryable(Of SubjectReport)
        Dim id As Integer = Me.Id
        Dim studentReps = Container.AllInstances(Of SubjectReport)().Where(Function(sr) sr.Student.Id = id)

        If forSubject IsNot Nothing Then
            Dim subjId = forSubject.Id
            studentReps = studentReps.Where(Function(sr) sr.Subject.Id = subjId)
        End If

        Return studentReps.OrderByDescending(Function(sr) sr.Date)
    End Function

    Public Function CreateNewReport(ByVal [sub] As Subject) As SubjectReport
        Dim rep = Container.CreateTransientInstance(Of SubjectReport)()
        rep.Student = Me
            rep.Subject = [sub]
            Return rep
        End Function

        Public Sub SendMessage(ByVal subject As String, ByVal message As String)
        Container.AddMessageToBroker("Message sent to " & FullName)
    End Sub

        Public Overrides Function ToString() As String
            Return FullName
        End Function
    End Class

