
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

    <DemoProperty(Order:=3)>
    Public Overridable Property Grade As Grades

    <DemoProperty(Order:=4)>
    Public Overridable Property GivenBy As Teacher

    Public Sub AboutGivenBy(a As FieldAbout)
        Select Case a.TypeCode
            Case AboutTypeCodes.Parameters
                a.Options = Container.AllInstances(Of Teacher).ToArray()
            Case Else
        End Select
    End Sub


    Public Property mappedDate As Date
    Private myDate As NODate

    <DemoProperty(Order:=5)>
    Public ReadOnly Property [Date] As NODate
        Get
            If Not Container.IsPersistent(Me) Then
                mappedDate = Today
            End If
            myDate = If(myDate, New NODate(mappedDate, Sub(v) mappedDate = v))
            Return myDate
        End Get
    End Property

    Public Property mappedNotes As String
    Private myNotes As MultiLineTextString

    <DemoProperty(Order:=6)>
    Public ReadOnly Property Notes As MultiLineTextString
        Get
            myNotes = If(myNotes, New MultiLineTextString(mappedNotes, Sub(v) mappedNotes = v))
            Return myNotes
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Subject} {[Date]}"
    End Function

    Public Function Title() As Title
        Return New Title()
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

