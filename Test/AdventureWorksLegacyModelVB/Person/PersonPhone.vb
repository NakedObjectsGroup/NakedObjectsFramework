Namespace AW.Types

	Partial Public Class PersonPhone

        Public Property BusinessEntityID() As Integer

#Region "PhoneNumber"
        Friend mappedPhoneNumber As String
        Friend myPhoneNumber As TextString

        '<MemberOrder(1)>
        Public ReadOnly Property PhoneNumber As TextString
            Get
                Return If(myPhoneNumber, New TextString(mappedPhoneNumber, Function(v) mappedPhoneNumber = v))
            End Get
        End Property

        Public Sub AboutPhoneNumber(a As FieldAbout, PhoneNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        Public Property PhoneNumberTypeID() As Integer

        Public Overridable Property PhoneNumberType() As PhoneNumberType

#Region "ModifiedDate"
        Friend mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout, ModifiedDate As TimeStamp)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Function Title() As Title
            Return New Title($"{PhoneNumberType}:{PhoneNumber}")
        End Function
    End Class
End Namespace