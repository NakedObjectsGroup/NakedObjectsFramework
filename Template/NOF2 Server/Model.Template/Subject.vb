
Imports NOF2.Interface

Public Class Subject
    Implements IBounded
    Public Property Container As IContainer

    Public Overridable Property Id As Integer
    <DemoProperty(Order:=1)>
    Public Overridable Property Name As String

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class

