Namespace AW.Types

	Partial Public Class Person
		Inherits BusinessEntity
		Implements IHasRowGuid, IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As Person)
			MyBase.New(cloneFrom)
			NameStyle = cloneFrom.NameStyle
			Title = cloneFrom.Title
			FirstName = cloneFrom.FirstName
			MiddleName = cloneFrom.MiddleName
			LastName = cloneFrom.LastName
			Suffix = cloneFrom.Suffix
			PersonType = cloneFrom.PersonType
			EmailPromotion = cloneFrom.EmailPromotion
			EmailAddresses = cloneFrom.EmailAddresses
			PhoneNumbers = cloneFrom.PhoneNumbers
			Password = cloneFrom.Password
			AdditionalContactInfo = cloneFrom.AdditionalContactInfo
			Employee = cloneFrom.Employee
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub

#Region "Name fields"

		<MemberOrder(15)>
		<Named("Reverse name order")>
		Public Property NameStyle() As Boolean

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Title {get;set;}
		<MemberOrder(11)>
		Public Property Title() As String

		<MemberOrder(12)>
		Public Property FirstName() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? MiddleName {get;set;}
		<MemberOrder(13)>
		Public Property MiddleName() As String

		<MemberOrder(14)>
		Public Property LastName() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Suffix {get;set;}
		<MemberOrder(15)>
		Public Property Suffix() As String

#End Region

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? PersonType {get;set;}
		<Hidden>
		Public Property PersonType() As String

		<MemberOrder(21)>
		<Hidden>
		Public Overridable Property EmailPromotion() As EmailPromotion

		'To test a null image
		'[NotMapped]
		'public virtual Image Photo { get { return null; } }

		<TableView(False, NameOf(EmailAddress.EmailAddress1))>
		Public Overridable Property EmailAddresses() As ICollection(Of EmailAddress) = New List(Of EmailAddress)()

		<AWNotCounted>
		<TableView(False, NameOf(PersonPhone.PhoneNumberType), NameOf(PersonPhone.PhoneNumber))>
		Public Overridable Property PhoneNumbers() As ICollection(Of PersonPhone) = New List(Of PersonPhone)()

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Password? Password {get;set;}
		Public Overridable Property Password() As Password

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? AdditionalContactInfo {get;set;}
		<MemberOrder(30)>
		Public Property AdditionalContactInfo() As String

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Employee? Employee {get;set;}
		<Hidden>
		Public Overridable Property Employee() As Employee

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return If(NameStyle, $"{LastName} {FirstName}", $"{FirstName} {LastName}")
		End Function
	End Class
End Namespace