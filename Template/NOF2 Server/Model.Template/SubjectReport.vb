
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations



Public Class SubjectReport
    Public Property Container As IContainer
    Public Property Students As StudentRepository
    Public Property Teachers As TeacherRepository
    Public Property Subjects As SubjectRepository

    Public Overridable Property Id As Integer

    <DemoProperty(Order:=1)> 'Disabled
    Public Overridable Property Student As Student


    Public Function AutoCompleteStudent(
        <MinLength(2)> ByVal matching As String) As IQueryable(Of Student)
        Return Students.FindStudentByName(matching)
    End Function

    <DemoProperty(Order:=2)>
    Public Overridable Property Subject As Subject

    Public Function ChoicesSubject() As IList(Of Subject)
        Return Subjects.AllSubjects().ToList()
    End Function

    <DemoProperty(Order:=3)>
    Public Overridable Property Grade As Grades
    <DemoProperty(Order:=4)>
    Public Overridable Property GivenBy As Teacher

    Public Function ChoicesGivenBy() As IList(Of Teacher)
        Return Teachers.AllTeachers().ToList()
    End Function

    <DemoProperty(Order:=5)>
    Public Overridable Property [Date] As DateTime

    Public Function DefaultDate() As DateTime
        Return DateTime.Today
    End Function


    '! MUltiline
    <DemoProperty(Order:=6)>
    Public Overridable Property Notes As String

    Public Overrides Function ToString() As String
        Return $"{Subject} {[Date].ToString("d")}"
    End Function
End Class

Public Enum Grades
    A_star
    A
    B
    C
    D
    E
    U
End Enum

