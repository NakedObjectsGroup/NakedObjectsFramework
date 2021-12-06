Namespace AW.Types

	Public MustInherit Class BusinessEntity

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<Hidden>
		Public Property BusinessEntityRowguid() As Guid

		<Hidden>
		Public Property BusinessEntityModifiedDate() As DateTime

		<TableView(False, NameOf(BusinessEntityAddress.AddressType), NameOf(BusinessEntityAddress.Address))>
		Public Overridable Property Addresses() As ICollection(Of BusinessEntityAddress) = New List(Of BusinessEntityAddress)()

		<TableView(False, NameOf(BusinessEntityContact.ContactType), NameOf(BusinessEntityContact.Person))>
		Public Overridable Property Contacts() As ICollection(Of BusinessEntityContact) = New List(Of BusinessEntityContact)()
	End Class
End Namespace