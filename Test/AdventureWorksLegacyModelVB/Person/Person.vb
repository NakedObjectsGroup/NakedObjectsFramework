Imports AW.Services

Namespace AW.Types

    Partial Public Class Person
        Inherits BusinessEntity
        Implements ITitledObject

#Region "Name fields"
#Region "NameStyle"
        Public Property mappedNameStyle As Boolean
        Friend myNameStyle As Logical

        <MemberOrder(15)>
        Public ReadOnly Property NameStyle As Logical
            Get
                myNameStyle = If(myNameStyle, New Logical(mappedNameStyle, Sub(v) mappedNameStyle = v))
Return myNameStyle
            End Get
        End Property

        Public Sub AboutNameStyle(a As FieldAbout, NameStyle As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Reverse name order"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Title"
        Public Property mappedTitle As String
        Friend myTitle As TextString

        <MemberOrder(1)>
        Public ReadOnly Property NameTitle As TextString
            Get
                myTitle = If(myTitle, New TextString(mappedTitle, Sub(v) mappedTitle = v))
Return myTitle
            End Get
        End Property

        Public Sub AboutTitle(a As FieldAbout, Title As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Title"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "FirstName"
        Public Property mappedFirstName As String
        Friend myFirstName As TextString

        <MemberOrder(12)>
        Public ReadOnly Property FirstName As TextString
            Get
                myFirstName = If(myFirstName, New TextString(mappedFirstName, Sub(v) mappedFirstName = v))
Return myFirstName
            End Get
        End Property

        Public Sub AboutFirstName(a As FieldAbout, FirstName As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "MiddleName"
        Public Property mappedMiddleName As String
        Friend myMiddleName As TextString

        <MemberOrder(13)>
        Public ReadOnly Property MiddleName As TextString
            Get
                myMiddleName = If(myMiddleName, New TextString(mappedMiddleName, Sub(v) mappedMiddleName = v))
Return myMiddleName
            End Get
        End Property

        Public Sub AboutMiddleName(a As FieldAbout, MiddleName As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "LastName"
        Public Property mappedLastName As String
        Friend myLastName As TextString

        <MemberOrder(14)>
        Public ReadOnly Property LastName As TextString
            Get
                myLastName = If(myLastName, New TextString(mappedLastName, Sub(v) mappedLastName = v))
Return myLastName
            End Get
        End Property

        Public Sub AboutLastName(a As FieldAbout, LastName As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Suffix"
        Public Property mappedSuffix As String
        Friend mySuffix As TextString

        <MemberOrder(15)>
        Public ReadOnly Property Suffix As TextString
            Get
                mySuffix = If(mySuffix, New TextString(mappedSuffix, Sub(v) mappedSuffix = v))
Return mySuffix
            End Get
        End Property

        Public Sub AboutSuffix(a As FieldAbout, Suffix As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region
#End Region

#Region "PersonType"
        Public Property mappedPersonType As String
        Friend myPersonType As TextString

        <MemberOrder(1)>
        Public ReadOnly Property PersonType As TextString
            Get
                myPersonType = If(myPersonType, New TextString(mappedPersonType, Sub(v) mappedPersonType = v))
Return myPersonType
            End Get
        End Property

        Public Sub AboutPersonType(a As FieldAbout, PersonType As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
                    a.Visible = False
            End Select
        End Sub
#End Region

#Region "EmailPromotion"
        <MemberOrder(21)>
        Public Overridable Property EmailPromotion() As EmailPromotion

        Public Sub AboutEmailPromotion(a As FieldAbout, ep As EmailPromotion)
            Select Case a.TypeCode
                Case Else
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
                myEmailAddresses = If(myEmailAddresses, New InternalCollection(Of EmailAddress)(mappedEmailAddresses))
Return myEmailAddresses
            End Get
        End Property

        Public Sub AboutEmailAddresses(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

#Region "PhoneNumbers (Collection)"
        Public Overridable Property mappedPhoneNumbers As ICollection(Of PersonPhone) = New List(Of PersonPhone)()

        Private myPhoneNumbers As InternalCollection

        ''<TableView(False, NameOf(PersonPhone.PhoneNumberType), NameOf(PersonPhone.PhoneNumber))>
        Public ReadOnly Property PhoneNumbers As InternalCollection
            Get
                myPhoneNumbers = If(myPhoneNumbers, New InternalCollection(Of PersonPhone)(mappedPhoneNumbers))
Return myPhoneNumbers
            End Get
        End Property

        Public Sub AboutPhoneNumbers(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

        Public Overridable Property Password() As Password

#Region "AdditionalContactInfo"
        Public Property mappedAdditionalContactInfo As String
        Friend myAdditionalContactInfo As TextString

        <MemberOrder(30)>
        Public ReadOnly Property AdditionalContactInfo As TextString
            Get
                myAdditionalContactInfo = If(myAdditionalContactInfo, New TextString(mappedAdditionalContactInfo, Sub(v) mappedAdditionalContactInfo = v))
Return myAdditionalContactInfo
            End Get
        End Property

        Public Sub AboutAdditionalContactInfo(a As FieldAbout, AdditionalContactInfo As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        Public Overridable Property Employee() As Employee

        Public Sub AboutEmployee(a As FieldAbout, e As Employee)
            Select Case a.TypeCode
                Case Else
                    a.Visible = False
            End Select
        End Sub

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
Return myModifiedDate
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Property RowGuid() As Guid

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return If(mappedNameStyle, $"{LastName} {FirstName}", $"{FirstName} {LastName}")
        End Function


#Region "Actions"
        Public Function ActionOthersWithSameInitials() As IQueryable(Of Person)
            Dim rep = CType(Container.Repository(GetType(PersonRepository)), PersonRepository)
            Return rep.FindContactByName(mappedFirstName.Substring(0, 1), mappedLastName.Substring(0, 1))
        End Function

#End Region
    End Class
End Namespace