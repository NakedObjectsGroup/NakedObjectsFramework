Namespace AW.Types

	<Bounded>
	Partial Public Class PhoneNumberType
		Implements IHasModifiedDate

		<Hidden>
		Public Property PhoneNumberTypeID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Name {get;set;}
		<Hidden>
		Public Property Name() As String

		<Hidden>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public override string? ToString()
		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace