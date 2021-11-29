Namespace AW.Types

	<Bounded>
	Partial Public Class AddressType
		Implements IHasModifiedDate, IHasRowGuid

		<Hidden>
		Public Property AddressTypeID() As Integer

		<Hidden>
		Public Property Name() As String = ""


		<Hidden>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace