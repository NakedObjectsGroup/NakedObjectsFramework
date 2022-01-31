
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations



Public Class SubjectReport
    Implements IContainerAware
    Public Property Container As IContainer Implements IContainerAware.Container

    Public Overridable Property Id As Integer

    <DemoProperty(Order:=1)> 'Disabled
    Public Overridable Property Student As Student

    <DemoProperty(Order:=2)>
    Public Overridable Property Subject As Subject

    'TODO
    Public Function ChoicesSubject() As IList(Of Subject)
        Return Container.AllInstances(Of Subject).ToList()
    End Function

    <DemoProperty(Order:=3)>
    Public Overridable Property Grade As Grades

    <DemoProperty(Order:=4)>
    Public Overridable Property GivenBy As Teacher

    'TODO
    Public Function ChoicesGivenBy() As IList(Of Teacher)
        Return Container.AllInstances(Of Teacher).ToList()
    End Function


    Public mappedDate As Date
    Private myDate As NODate

    <DemoProperty(Order:=5)>
    Public ReadOnly Property [Date] As NODate
        Get
            myDate = If(myDate, New NODate(mappedDate, Function(v) mappedDate = v))
            Return myDate
        End Get
    End Property

    'TODO
    Public Function DefaultDate() As DateTime
        Return DateTime.Today
    End Function

    Public mappedNotes As String
    Private myNotes As MultiLineTextString

    <DemoProperty(Order:=6)>
    Public ReadOnly Property Notes As MultiLineTextString
        Get
            myNotes = If(myNotes, New MultiLineTextString(mappedNotes, Function(v) mappedNotes = v))
            Return myNotes
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Subject} {[Date].ToString("d")}"
    End Function

    Public Function Title() As Title
        Return New Title(ToString())
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

