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

		
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"{PhoneNumberType}:{PhoneNumber}"
		End Function
	End Class
End Namespace