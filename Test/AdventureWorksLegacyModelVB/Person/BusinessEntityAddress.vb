Namespace AW.Types

	<Named("Address")>
	Partial Public Class BusinessEntityAddress
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<MemberOrder(3)>
		Public Overridable Property BusinessEntity() As BusinessEntity


		<Hidden>
		Public Property AddressTypeID() As Integer

		<MemberOrder(1)>
		Public Overridable Property AddressType() As AddressType

		<Hidden>
		Public Property AddressID() As Integer


		<MemberOrder(2)>
		Public Overridable Property Address() As Address


		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return $"{AddressType}: {Address}"
		End Function
	End Class
End Namespace