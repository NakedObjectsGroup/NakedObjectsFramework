Namespace AW.Types

	Partial Public Class Person
		Inherits BusinessEntity
		Implements ITitledObject

#Region "Name fields"
#Region "NameStyle"
		Public mappedNameStyle As Boolean
		Friend myNameStyle As Logical

		'<MemberOrder(15)>
		Public ReadOnly Property NameStyle As Logical
			Get
				Return If(myNameStyle, New Logical(mappedNameStyle, Function(v) mappedNameStyle = v))
			End Get
		End Property

		Public Sub AboutNameStyle(a As FieldAbout, NameStyle As Logical)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Reverse name order"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Title"
		Public mappedTitle As String
		Friend myTitle As TextString

		'<MemberOrder(1)>
		Public ReadOnly Property NameTitle As TextString
			Get
				Return If(myTitle, New TextString(mappedTitle, Function(v) mappedTitle = v))
			End Get
		End Property

		Public Sub AboutTitle(a As FieldAbout, Title As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Title"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "FirstName"
		Public mappedFirstName As String
		Friend myFirstName As TextString

		'<MemberOrder(12)>
		Public ReadOnly Property FirstName As TextString
			Get
				Return If(myFirstName, New TextString(mappedFirstName, Function(v) mappedFirstName = v))
			End Get
		End Property

		Public Sub AboutFirstName(a As FieldAbout, FirstName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "MiddleName"
		Public mappedMiddleName As String
		Friend myMiddleName As TextString

		'<MemberOrder(13)>
		Public ReadOnly Property MiddleName As TextString
			Get
				Return If(myMiddleName, New TextString(mappedMiddleName, Function(v) mappedMiddleName = v))
			End Get
		End Property

		Public Sub AboutMiddleName(a As FieldAbout, MiddleName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "LastName"
		Public mappedLastName As String
		Friend myLastName As TextString

		'<MemberOrder(14)>
		Public ReadOnly Property LastName As TextString
			Get
				Return If(myLastName, New TextString(mappedLastName, Function(v) mappedLastName = v))
			End Get
		End Property

		Public Sub AboutLastName(a As FieldAbout, LastName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Suffix"
		Public mappedSuffix As String
		Friend mySuffix As TextString

		'<MemberOrder(15)>
		Public ReadOnly Property Suffix As TextString
			Get
				Return If(mySuffix, New TextString(mappedSuffix, Function(v) mappedSuffix = v))
			End Get
		End Property

		Public Sub AboutSuffix(a As FieldAbout, Suffix As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region
#End Region

#Region "PersonType"
		Public mappedPersonType As String
		Friend myPersonType As TextString

		'<MemberOrder(1)>
		Public ReadOnly Property PersonType As TextString
			Get
				Return If(myPersonType, New TextString(mappedPersonType, Function(v) mappedPersonType = v))
			End Get
		End Property

		Public Sub AboutPersonType(a As FieldAbout, PersonType As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub
#End Region

#Region "EmailPromotion"
		'<MemberOrder(21)>
		Public Overridable Property EmailPromotion() As EmailPromotion

		Public Sub AboutEmailPromotion(a As FieldAbout, ep As EmailPromotion)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub
#End Region

		'To test a null image
		'[NotMapped]
		'public virtual Image Photo { get { return null; } }

#Region "EmailAddresses (Collection)"
		Public Overridable Property mappedEmailAddresses As ICollection(Of EmailAddress) = New List(Of EmailAddress)()

		Private myEmailAddresses As InternalCollection

		'		'<TableView(False, NameOf(EmailAddress.EmailAddress1))>
		Public ReadOnly Property EmailAddresses As InternalCollection
			Get
				Return If(myEmailAddresses, New InternalCollection(Of EmailAddress)(mappedEmailAddresses))
			End Get
		End Property

		Public Sub AboutEmailAddresses(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "PhoneNumbers (Collection)"
		Public Overridable Property mappedPhoneNumbers As ICollection(Of PersonPhone) = New List(Of PersonPhone)()

		Private myPhoneNumbers As InternalCollection

		''<TableView(False, NameOf(PersonPhone.PhoneNumberType), NameOf(PersonPhone.PhoneNumber))>
		Public ReadOnly Property PhoneNumbers As InternalCollection
			Get
				Return If(myPhoneNumbers, New InternalCollection(Of PersonPhone)(mappedPhoneNumbers))
			End Get
		End Property

		Public Sub AboutPhoneNumbers(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Overridable Property Password() As Password

#Region "AdditionalContactInfo"
		Public mappedAdditionalContactInfo As String
		Friend myAdditionalContactInfo As TextString

		'<MemberOrder(30)>
		Public ReadOnly Property AdditionalContactInfo As TextString
			Get
				Return If(myAdditionalContactInfo, New TextString(mappedAdditionalContactInfo, Function(v) mappedAdditionalContactInfo = v))
			End Get
		End Property

		Public Sub AboutAdditionalContactInfo(a As FieldAbout, AdditionalContactInfo As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Overridable Property Employee() As Employee

		Public Sub AboutEmployee(a As FieldAbout, e As Employee)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
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

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(If(mappedNameStyle, $"{LastName} {FirstName}", $"{FirstName} {LastName}"))
		End Function
	End Class
End Namespace