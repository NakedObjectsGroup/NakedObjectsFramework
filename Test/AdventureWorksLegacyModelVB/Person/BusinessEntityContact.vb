Namespace AW.Types
	<Named("Contact")>
	Partial Public Class BusinessEntityContact
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property BusinessEntityID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual BusinessEntity? BusinessEntity {get;set;}
		<Hidden>
		Public Overridable Property BusinessEntity() As BusinessEntity

		<Hidden>
		Public Property PersonID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Person? Person {get;set;}
		<MemberOrder(1)>
		Public Overridable Property Person() As Person

		<Hidden>
		Public Property ContactTypeID() As Integer

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual ContactType? ContactType {get;set;}
		<MemberOrder(2)>
		Public Overridable Property ContactType() As ContactType


		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return $"{Person}"
		End Function
	End Class
End Namespace