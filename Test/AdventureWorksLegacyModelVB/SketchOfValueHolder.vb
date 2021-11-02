Public Class SketchOfValueHolder

    Friend mappedName As String

    Private myName As TextString = If(myName, New TextString(mappedName, Sub(s) mappedName = s))

    Public ReadOnly Property Name As TextString
        Get
            Return myName
        End Get
    End Property

End Class

Public Class TextString

    Private value As String

    Private updateMappedField As Action(Of String)

    Public Sub New(value As String, updateBackingField As Action(Of String))
        Me.value = value
        updateMappedField = updateBackingField
    End Sub


    Public Sub SetValue(newValue As String)
        value = newValue
        updateMappedField(newValue)
    End Sub

    Public Function GetValue() As String
        Return value
    End Function


End Class
