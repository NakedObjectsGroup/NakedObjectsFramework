Namespace AW.Types

	Partial Public Class Person
		Inherits BusinessEntity
		Implements IHasRowGuid, IHasModifiedDate

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

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return If(NameStyle, $"{LastName} {FirstName}", $"{FirstName} {LastName}")
		End Function
	End Class
End Namespace