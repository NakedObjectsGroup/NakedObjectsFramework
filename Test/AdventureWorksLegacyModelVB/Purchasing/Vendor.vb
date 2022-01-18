Namespace AW.Types

    Partial Public Class Vendor

        Implements ITitledObject

        Public Shared Function FieldOrder() As String
            Return "Accountnumber, NAME,  CreditRating, PreferredVendorStatus, " +
            "ActiveFlag,purchasingWebServiceURL, ModifiedDate, Products"
            'spacing and casing for testing
        End Function

#Region "Name"
        Public Property mappedName As String
        Friend myName As TextString


        Public ReadOnly Property Name As TextString
            Get
                Return If(myName, New TextString(mappedName, Sub(v) mappedName = v))
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
        Public Property mappedAccountNumber As String
        Friend myAccountNumber As TextString


        Public ReadOnly Property AccountNumber As TextString
            Get
                Return If(myAccountNumber, New TextString(mappedAccountNumber, Sub(v) mappedAccountNumber = v))
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
        Public Property mappedCreditRating As Byte
        Friend myCreditRating As WholeNumber


        Public ReadOnly Property CreditRating As WholeNumber
            Get
                Return If(myCreditRating, New WholeNumber(mappedCreditRating, Sub(v) mappedCreditRating = CType(v, Byte)))
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
        Public Property mappedPreferredVendorStatus As Boolean
        Friend myPreferredVendorStatus As Logical


        Public ReadOnly Property PreferredVendorStatus As Logical
            Get
                Return If(myPreferredVendorStatus, New Logical(mappedPreferredVendorStatus, Sub(v) mappedPreferredVendorStatus = v))
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
        Public Property mappedActiveFlag As Boolean
        Friend myActiveFlag As Logical


        Public ReadOnly Property ActiveFlag As Logical
            Get
                Return If(myActiveFlag, New Logical(mappedActiveFlag, Sub(v) mappedActiveFlag = v))
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
        Public Property mappedPurchasingWebServiceURL As String
        Friend myPurchasingWebServiceURL As TextString


        Public ReadOnly Property PurchasingWebServiceURL As TextString
            Get
                Return If(myPurchasingWebServiceURL, New TextString(mappedPurchasingWebServiceURL, Sub(v) mappedPurchasingWebServiceURL = v))
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
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp


        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        '<Hidden>
        Public Property BusinessEntityID() As Integer


        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function
    End Class
End Namespace