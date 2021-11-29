Namespace AW.Types

	Partial Public Class EmailAddress
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<Hidden>
		Public Property EmailAddressID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? EmailAddress1 {get;set;}
		<Named("Email Address")>
		Public Property EmailAddress1() As String


		<Hidden>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public override string? ToString()
		Public Overrides Function ToString() As String
			Return EmailAddress1
		End Function
	End Class
End Namespace