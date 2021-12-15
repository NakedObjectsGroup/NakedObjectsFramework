Namespace AW.Types

    Partial Public Class Vendor

#Region "AccountNumber"
        Friend mappedAccountNumber As String
        Friend myAccountNumber As TextString

        '<MemberOrder(10)>
        Public ReadOnly Property AccountNumber As TextString
            Get
                Return If(myAccountNumber, New TextString(mappedAccountNumber, Function(v) mappedAccountNumber = v))
            End Get
        End Property

        Public Sub AboutAccountNumber(a As FieldAbout, AccountNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Name"
        Friend mappedName As String
        Friend myName As TextString

        '<MemberOrder(20)>
        Public ReadOnly Property Name As TextString
            Get
                Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "CreditRating"
        Friend mappedCreditRating As Byte
        Friend myCreditRating As WholeNumber

        '<MemberOrder(30)>
        Public ReadOnly Property CreditRating As WholeNumber
            Get
                Return If(myCreditRating, New WholeNumber(mappedCreditRating, Function(v) mappedCreditRating = v))
            End Get
        End Property

        Public Sub AboutCreditRating(a As FieldAbout, CreditRating As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "PreferredVendorStatus"
        Friend mappedPreferredVendorStatus As Boolean
        Friend myPreferredVendorStatus As Logical

        '<MemberOrder(40)>
        Public ReadOnly Property PreferredVendorStatus As Logical
            Get
                Return If(myPreferredVendorStatus, New Logical(mappedPreferredVendorStatus, Function(v) mappedPreferredVendorStatus = v))
            End Get
        End Property

        Public Sub AboutPreferredVendorStatus(a As FieldAbout, PreferredVendorStatus As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ActiveFlag"
        Friend mappedActiveFlag As Boolean
        Friend myActiveFlag As Logical

        '<MemberOrder(50)>
        Public ReadOnly Property ActiveFlag As Logical
            Get
                Return If(myActiveFlag, New Logical(mappedActiveFlag, Function(v) mappedActiveFlag = v))
            End Get
        End Property

        Public Sub AboutActiveFlag(a As FieldAbout, ActiveFlag As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "PurchasingWebServiceURL"
        Friend mappedPurchasingWebServiceURL As String
        Friend myPurchasingWebServiceURL As TextString

        '<MemberOrder(60)>
        Public ReadOnly Property PurchasingWebServiceURL As TextString
            Get
                Return If(myPurchasingWebServiceURL, New TextString(mappedPurchasingWebServiceURL, Function(v) mappedPurchasingWebServiceURL = v))
            End Get
        End Property

        Public Sub AboutPurchasingWebServiceURL(a As FieldAbout, PurchasingWebServiceURL As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Products (Collection)"
        Public Overridable Property mappedProducts As ICollection(Of ProductVendor) = New List(Of ProductVendor)()

        Private myProducts As InternalCollection

        '<MemberOrder(1)>
        Public ReadOnly Property Products As InternalCollection
            Get
                Return If(myProducts, New InternalCollection(Of ProductVendor)(mappedProducts))
            End Get
        End Property

        Public Sub AboutProducts(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Product - Order Info"
            End Select
        End Sub
#End Region

#Region "ModifiedDate"
        Friend mappedModifiedDate As Date
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

        '<Hidden>
        Public Property BusinessEntityID() As Integer


        Public Function Title() As Title
            Return New Title(Name)
        End Function
    End Class
End Namespace