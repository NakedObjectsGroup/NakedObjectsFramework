﻿
Imports NOF2.Interface

Public Class Subject
    Implements IBounded

    Public Overridable Property Id As Integer

    Public mappedName As String
    Private myName As TextString

    <DemoProperty(Order:=1)>
    Public ReadOnly Property Name As TextString
        Get
            myName = If(myName, New TextString(mappedName, Function(v) mappedName = v))
            Return myName
        End Get
    End Property

    Public Function Title() As Title
        Return New Title(Name)
    End Function

End Class
