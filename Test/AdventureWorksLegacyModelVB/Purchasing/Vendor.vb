Namespace AW.Types

    Partial Public Class Vendor

        Public Shared Function FieldOrder() As String
            Return "AccountNumber, Name, CreditRating, PreferredVendorStatus, " +
            "ActiveFlag, PurchasingWebServiceURL, ModifiedDate, Products"
        End Function

#Region "Name"
        Public mappedName As String
        Friend myName As TextString


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

#Region "AccountNumber"
        Public mappedAccountNumber As String
        Friend myAccountNumber As TextString


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

#Region "CreditRating"
        Public mappedCreditRating As Byte
        Friend myCreditRating As WholeNumber


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
        Public mappedPreferredVendorStatus As Boolean
        Friend myPreferredVendorStatus As Logical


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
        Public mappedActiveFlag As Boolean
        Friend myActiveFlag As Logical


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
        Public mappedPurchasingWebServiceURL As String
        Friend myPurchasingWebServiceURL As TextString


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
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp


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