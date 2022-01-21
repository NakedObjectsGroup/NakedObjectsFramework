Namespace AW.Types

    Partial Public Class PersonPhone

        Implements ITitledObject

        Public Property BusinessEntityID() As Integer

        Public Overridable Property Person As Person

#Region "PhoneNumber"
        Public Property mappedPhoneNumber As String
        Friend myPhoneNumber As TextString

        <MemberOrder(1)>
        Public ReadOnly Property PhoneNumber As TextString
            Get
                myPhoneNumber = If(myPhoneNumber, New TextString(mappedPhoneNumber, Sub(v) mappedPhoneNumber = v))
Return myPhoneNumber
            End Get
        End Property

        Public Sub AboutPhoneNumber(a As FieldAbout, PhoneNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        Public Property PhoneNumberTypeID() As Integer

        Public Overridable Property PhoneNumberType() As PhoneNumberType

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
Return myModifiedDate
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"{PhoneNumberType}:{PhoneNumber}"
        End Function
    End Class
End Namespace