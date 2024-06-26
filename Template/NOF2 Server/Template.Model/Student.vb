﻿Public Class Student
    Implements IContainerAware

    Public Property Container As IContainer Implements IContainerAware.Container

    Public Overridable Property Id As Integer

    Public Property mappedFullName As String
    Private myFullName As TextString

    <DemoProperty(Order:=1, IsRequired:=True)>
    Public ReadOnly Property FullName As TextString
        Get
            myFullName = If(myFullName, New TextString(mappedFullName, Sub(v) mappedFullName = v))
            Return myFullName
        End Get
    End Property

    Public Property mappedCurrentYearGroup As Integer
    Private myCurrentYearGroup As WholeNumber

    <DemoProperty(Order:=2, IsRequired:=True)>
    Public ReadOnly Property CurrentYearGroup As WholeNumber
        Get
            myCurrentYearGroup = If(myCurrentYearGroup, New WholeNumber(mappedCurrentYearGroup, Sub(v) mappedCurrentYearGroup = v))
            Return myCurrentYearGroup
        End Get
    End Property

    Public Sub AboutCurrentYearGroup(a As FieldAbout, yg As WholeNumber)
        Select Case a.TypeCode
            Case AboutTypeCodes.Valid
                If (yg.Value > 13 OrElse yg.Value < 9) Then
                    a.IsValid = False
                    a.InvalidReason = "Must be in range 9-13"
                End If
        End Select
    End Sub

    Public Property PersonalTutorId As Integer?

    <DemoProperty(Order:=4)>
    Public Overridable Property PersonalTutor As Teacher

    Public Sub AboutPersonalTutor(a As FieldAbout)
        Select Case a.TypeCode
            Case AboutTypeCodes.Parameters
                a.Options = Container.AllInstances(Of Teacher).ToArray()
            Case Else
        End Select
    End Sub

    Public Overridable Property mappedSets As ICollection(Of TeachingSet) = New List(Of TeachingSet)()

    Private mySets As InternalCollection

    <DemoProperty(Order:=3)>
    <TableView(False, "Subject", "SetName", "Teacher")>
    Public ReadOnly Property Sets As InternalCollection
        Get
            mySets = If(mySets, New InternalCollection(Of TeachingSet)(mappedSets))
            Return mySets
        End Get
    End Property

    Public Function ActionListRecentReports(forSubject As Subject) _
            As IQueryable(Of SubjectReport)
        Dim id As Integer = Me.Id
        Dim studentReps = Container.AllInstances(Of SubjectReport)().Where(Function(sr) sr.Student.Id = id)

        If forSubject IsNot Nothing Then
            Dim subjId = forSubject.Id
            studentReps = studentReps.Where(Function(sr) sr.Subject.Id = subjId)
        End If

        Return studentReps.OrderByDescending(Function(sr) sr.mappedDate)
    End Function

    Public Function ActionCreateNewReport(ByVal subject As Subject) As SubjectReport
        Dim rep = Container.CreateTransientInstance(Of SubjectReport)()
        rep.Student = Me
        rep.Subject = subject
        Return rep
    End Function

    Public Sub ActionSendMessage(ByVal subject As TextString, ByVal message As TextString)
        Container.AddMessageToBroker($"Message sent to {FullName}")
    End Sub

    Public Sub ActionSave()
        Container.MakePersistent(Me)
    End Sub

    Public Function Title() As Title
        Return New Title(FullName)
    End Function
End Class

