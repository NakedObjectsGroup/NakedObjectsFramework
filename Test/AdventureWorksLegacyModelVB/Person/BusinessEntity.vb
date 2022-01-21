
Namespace AW.Types

    Public MustInherit Class BusinessEntity
        Implements IContainerAware

#Region "Container"
        Public Property Container As IContainer Implements IContainerAware.Container
#End Region

        Public Property BusinessEntityID() As Integer

        Public Property BusinessEntityRowguid() As Guid

        Public Property BusinessEntityModifiedDate() As DateTime

        Public Sub AboutBusinessEntityModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case Else
                    a.Visible = False
            End Select
        End Sub

#Region "Addresses (Collection)"
        Public Overridable Property mappedAddresses As ICollection(Of BusinessEntityAddress) = New List(Of BusinessEntityAddress)()

        Private myAddresses As InternalCollection

        ''<TableView(False, NameOf(BusinessEntityAddress.AddressType), NameOf(BusinessEntityAddress.Address))>
        Public ReadOnly Property Addresses As InternalCollection
            Get
                myAddresses = If(myAddresses, New InternalCollection(Of BusinessEntityAddress)(mappedAddresses))
                Return myAddresses
            End Get
        End Property

        Public Sub AboutAddresses(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region
#Region "Contacts (Collection)"
        Public Overridable Property mappedContacts As ICollection(Of BusinessEntityContact) = New List(Of BusinessEntityContact)()

        Private myContacts As InternalCollection

        ''<TableView(False, NameOf(BusinessEntityContact.ContactType), NameOf(BusinessEntityContact.Person))>
        Public ReadOnly Property Contacts As InternalCollection
            Get
                myContacts = If(myContacts, New InternalCollection(Of BusinessEntityContact)(mappedContacts))
                Return myContacts
            End Get
        End Property

        Public Sub AboutContacts(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

    End Class
End Namespace