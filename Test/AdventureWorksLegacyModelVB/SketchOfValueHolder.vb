Public Class SketchOfValueHolder

    Friend mappedName As String

    Private myName As TextString

    Public ReadOnly Property Name As TextString
        Get
            Return If(myName, New TextString(mappedName, Sub(s) mappedName = s))
        End Get
    End Property

End Class

Public Class TextString

    Private value As String

    Private updateMappedField As Action(Of String)

    Public Sub New(value As String, updateMappedField As Action(Of String))
        Me.value = value
        Me.updateMappedField = updateMappedField
    End Sub

    Public Sub SetValue(newValue As String)
        value = newValue
        updateMappedField(newValue)
    End Sub

    Public Function GetValue() As String
        Return value
    End Function

End Class
