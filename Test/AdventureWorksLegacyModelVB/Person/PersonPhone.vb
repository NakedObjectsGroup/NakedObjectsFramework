Namespace AW.Types

	Partial Public Class PersonPhone
		Implements IHasModifiedDate

		<Hidden>
		Public Property BusinessEntityID() As Integer

'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
'ORIGINAL LINE: public string? PhoneNumber {get;set;}
		Public Property PhoneNumber() As String

		<Hidden>
		Public Property PhoneNumberTypeID() As Integer

'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
'ORIGINAL LINE: public virtual PhoneNumberType? PhoneNumberType {get;set;}
		Public Overridable Property PhoneNumberType() As PhoneNumberType

#Region "ModifiedDate"
        Friend mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(1)>
        Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
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

        Public Overrides Function ToString() As String
			Return $"{PhoneNumberType}:{PhoneNumber}"
		End Function
	End Class
End Namespace