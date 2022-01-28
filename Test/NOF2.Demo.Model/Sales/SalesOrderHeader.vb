Namespace AW.Types

    '<Named("Sales Order")>
    Partial Public Class SalesOrderHeader
        Implements ITitledObject, INotEditableOncePersistent

        ''<Hidden>
        Public Property SalesOrderID() As Integer

#Region "SalesOrderNumber"
        Public Property mappedSalesOrderNumber As String
        Friend mySalesOrderNumber As TextString

        <DemoProperty(Order:=1)>
        Public ReadOnly Property SalesOrderNumber As TextString
            Get
                mySalesOrderNumber = If(mySalesOrderNumber, New TextString(mappedSalesOrderNumber, Sub(v) mappedSalesOrderNumber = v))
                Return mySalesOrderNumber
            End Get
        End Property

        Public Sub AboutSalesOrderNumber(a As FieldAbout, SalesOrderNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Status"
        Public Property Status() As Byte

        <DemoProperty(Order:=1)>
        Public Overridable ReadOnly Property StatusByte() As TextString
            Get
                Return New TextString([Enum].GetName(GetType(OrderStatus), Status))
            End Get
        End Property

        Public Sub AboutStatusByte(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Status"
            End Select
        End Sub
#End Region

#Region "Customer"
        ''<Hidden>
        Public Property CustomerID() As Integer

        <DemoProperty(Order:=2)>
        Public Overridable Property Customer() As Customer
#End Region

#Region "BillingAddress"
        ''<Hidden>
        Public Property BillingAddressID() As Integer

        <DemoProperty(Order:=4)>
        Public Overridable Property BillingAddress() As Address
#End Region
#Region "PurchaseOrderNumber"
        Public Property mappedPurchaseOrderNumber As String
        Friend myPurchaseOrderNumber As TextString

        <DemoProperty(Order:=5)>
        Public ReadOnly Property PurchaseOrderNumber As TextString
            Get
                myPurchaseOrderNumber = If(myPurchaseOrderNumber, New TextString(mappedPurchaseOrderNumber, Sub(v) mappedPurchaseOrderNumber = v))
                Return myPurchaseOrderNumber
            End Get
        End Property

        Public Sub AboutPurchaseOrderNumber(a As FieldAbout, PurchaseOrderNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Property ShippingAddressID() As Integer

        <DemoProperty(Order:=10)>
        Public Overridable Property ShippingAddress() As Address

        ''<Hidden>
        Public Property ShipMethodID() As Integer

        <DemoProperty(Order:=11)>
        Public Overridable Property ShipMethod() As ShipMethod

#Region "AccountNumber"
        Public Property mappedAccountNumber As String
        Friend myAccountNumber As TextString

        <DemoProperty(Order:=12)>
        Public ReadOnly Property AccountNumber As TextString
            Get
                myAccountNumber = If(myAccountNumber, New TextString(mappedAccountNumber, Sub(v) mappedAccountNumber = v))
                Return myAccountNumber
            End Get
        End Property

        Public Sub AboutAccountNumber(a As FieldAbout, AccountNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "OrderDate"
        Public Property mappedOrderDate As Date
        Friend myOrderDate As NODate

        <DemoProperty(Order:=20)>
        Public ReadOnly Property OrderDate As NODate
            Get
                myOrderDate = If(myOrderDate, New NODate(mappedOrderDate, Sub(v) mappedOrderDate = v))
                Return myOrderDate
            End Get
        End Property

        Public Sub AboutOrderDate(a As FieldAbout, OrderDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "DueDate"
        Public Property mappedDueDate As Date
        Friend myDueDate As NODate

        <DemoProperty(Order:=21)>
        Public ReadOnly Property DueDate As NODate
            Get
                myDueDate = If(myDueDate, New NODate(mappedDueDate, Sub(v) mappedDueDate = v))
                Return myDueDate
            End Get
        End Property

        Public Sub AboutDueDate(a As FieldAbout, DueDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ShipDate"
        Public Property mappedShipDate As Date?
        Friend myShipDate As NODateNullable

        <DemoProperty(Order:=22)>
        Public ReadOnly Property ShipDate As NODateNullable
            Get
                myShipDate = If(myShipDate, New NODateNullable(mappedShipDate, Sub(v) mappedShipDate = v))
                Return myShipDate
            End Get
        End Property

        Public Sub AboutShipDate(a As FieldAbout, ShipDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SubTotal"
        Public Property mappedSubTotal As Decimal
        Friend mySubTotal As Money

        <DemoProperty(Order:=31)>
        Public ReadOnly Property SubTotal As Money
            Get
                mySubTotal = If(mySubTotal, New Money(mappedSubTotal, Sub(v) mappedSubTotal = v))
                Return mySubTotal
            End Get
        End Property

        Public Sub AboutSubTotal(a As FieldAbout, SubTotal As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "TaxAmt"
        Public Property mappedTaxAmt As Decimal
        Friend myTaxAmt As Money

        <DemoProperty(Order:=32)>
        Public ReadOnly Property TaxAmt As Money
            Get
                myTaxAmt = If(myTaxAmt, New Money(mappedTaxAmt, Sub(v) mappedTaxAmt = v))
                Return myTaxAmt
            End Get
        End Property

        Public Sub AboutTaxAmt(a As FieldAbout, TaxAmt As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Freight"
        Public Property mappedFreight As Decimal
        Friend myFreight As Money

        <DemoProperty(Order:=33)>
        Public ReadOnly Property Freight As Money
            Get
                myFreight = If(myFreight, New Money(mappedFreight, Sub(v) mappedFreight = v))
                Return myFreight
            End Get
        End Property

        Public Sub AboutFreight(a As FieldAbout, Freight As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "TotalDue"
        Public Property mappedTotalDue As Decimal
        Friend myTotalDue As Money

        <DemoProperty(Order:=34)>
        Public ReadOnly Property TotalDue As Money
            Get
                myTotalDue = If(myTotalDue, New Money(mappedTotalDue, Sub(v) mappedTotalDue = v))
                Return myTotalDue
            End Get
        End Property

        Public Sub AboutTotalDue(a As FieldAbout, TotalDue As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Property CurrencyRateID() As Integer?

        <DemoProperty(Order:=35)>
        Public Overridable Property CurrencyRate() As CurrencyRate

#Region "OnlineOrder"
        Public Property mappedOnlineOrder As Boolean
        Friend myOnlineOrder As Logical

        <DemoProperty(Order:=41)>
        Public ReadOnly Property OnlineOrder As Logical
            Get
                myOnlineOrder = If(myOnlineOrder, New Logical(mappedOnlineOrder, Sub(v) mappedOnlineOrder = v))
                Return myOnlineOrder
            End Get
        End Property

        Public Sub AboutOnlineOrder(a As FieldAbout, OnlineOrder As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Online Order"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "CreditCard"

        Public Property CreditCardID As Integer?

        <DemoProperty(Order:=42)>
        Public Overridable Property CreditCard As CreditCard

#End Region

#Region "CreditCardApprovalCoad"
        Public Property mappedCreditCardApprovalCode As String
        Friend myCreditCardApprovalCode As TextString

        <DemoProperty(Order:=43)>
        Public ReadOnly Property CreditCardApprovalCode As TextString
            Get
                myCreditCardApprovalCode = If(myCreditCardApprovalCode, New TextString(mappedCreditCardApprovalCode, Sub(v) mappedCreditCardApprovalCode = v))
                Return myCreditCardApprovalCode
            End Get
        End Property

        Public Sub AboutCreditCardApprovalCode(a As FieldAbout, CreditCardApprovalCode As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                    a.Usable = False
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "RevisionNumber"
        Public Property mappedRevisionNumber As Byte
        Friend myRevisionNumber As WholeNumber

        <DemoProperty(Order:=51)>
        Public ReadOnly Property RevisionNumber As WholeNumber
            Get
                myRevisionNumber = If(myRevisionNumber, New WholeNumber(mappedRevisionNumber, Sub(v) mappedRevisionNumber = CType(v, Byte)))
                Return myRevisionNumber
            End Get
        End Property



        Public Sub AboutRevisionNumber(a As FieldAbout, RevisionNumber As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Comment"
        Public Property mappedComment As String
        Friend myComment As TextString
        '[MultiLine(NumberOfLines = 3, Width = 50)]
        <DemoProperty(Order:=52)>
        Public ReadOnly Property Comment As TextString
            Get
                myComment = If(myComment, New TextString(mappedComment, Sub(v) mappedComment = v))
                Return myComment
            End Get
        End Property

        Public Sub AboutComment(a As FieldAbout, Comment As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Description = "Free-form text"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
                    'comment is not null or empty

            End Select
        End Sub
#End Region

#Region "SalesPerson"

        Public Property SalesPersonID As Integer?

        <DemoProperty(Order:=62)>
        Public Overridable Property SalesPerson As SalesTerritory

#End Region

#Region "SalesTerritory"

        Public Property SalesTerritoryID As Integer?

        <DemoProperty(Order:=62)>
        Public Overridable Property SalesTerritory As SalesTerritory

#End Region

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <DemoProperty(Order:=99)>
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

#Region "Details (Collection)"
        Public Overridable Property mappedDetails As ICollection(Of SalesOrderDetail) = New List(Of SalesOrderDetail)()

        Private myDetails As InternalCollection

        <DemoProperty(Order:=1)>
        Public ReadOnly Property Details As InternalCollection
            Get
                myDetails = If(myDetails, New InternalCollection(Of SalesOrderDetail)(mappedDetails))
                Return myDetails
            End Get
        End Property

        Public Sub AboutDetails(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

#Region "Reasons (Collection)"
        Public Overridable Property mappedReasons As ICollection(Of SalesOrderHeaderSalesReason) = New List(Of SalesOrderHeaderSalesReason)()

        Private myReasons As InternalCollection

        <DemoProperty(Order:=1)>
        Public ReadOnly Property Reasons As InternalCollection
            Get
                myReasons = If(myReasons, New InternalCollection(Of SalesOrderHeaderSalesReason)(mappedReasons))
                Return myReasons
            End Get
        End Property

        Public Sub AboutReasons(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Property RowGuid() As Guid

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedSalesOrderNumber
        End Function

#Region "Actions"
        Public Sub ActionNoComment() 'Never visible
            Throw New NotImplementedException()
        End Sub

        Public Sub AboutActionNoComment(a As ActionAbout)
            Select Case a.TypeCode
                Case Else
                    a.Visible = False
            End Select
        End Sub

        Public Sub ActionClearComments()
            Me.Comment.Value = ""
        End Sub

        Public Sub AboutActionClearComments(a As ActionAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Description = "Delete all comments in Comment field"
                Case AboutTypeCodes.Usable
                    Dim c = Me.Comment.Value
                    If c Is Nothing OrElse c Is "" Then
                        a.Usable = False
                        a.UnusableReason = "Comment field is already clear"
                    End If
            End Select
        End Sub

        Public Sub ActionAppendComment(comment As TextString)
            Me.Comment.Value += $"; {comment}"
        End Sub

        Public Sub AboutActionAppendComment(a As ActionAbout, comment As TextString)
            Dim c = Me.Comment.Value
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Add Comment"
                    a.Description = "Append new comment to any existing"
                Case AboutTypeCodes.Parameters
                Case AboutTypeCodes.Usable
                    If Not c Is Nothing AndAlso c.Length > 45 Then
                        a.Usable = False
                        a.UnusableReason = "Existing comments already near capacity. Clear first"
                    End If
                Case AboutTypeCodes.Valid
                    If comment.IsEmpty Then
                        a.Usable = False
                        a.UnusableReason = "Comment cannot be empty"
                    ElseIf Not c Is Nothing AndAlso c.Length + comment.Value.Length > 50 Then
                        a.Usable = False
                        a.UnusableReason = "Total comment length would exceed 50 chars"
                    End If
                Case Else
            End Select
        End Sub


        Public Sub ActionAddNewDetail()
            Throw New NotImplementedException
        End Sub

        Public Sub ActionRemoveDetail()
            Throw New NotImplementedException
        End Sub
#End Region

    End Class

    Public Enum OrderStatus As Byte
        InProcess = 1
        Approved = 2
        BackOrdered = 3
        Rejected = 4
        Shipped = 5
        Cancelled = 6
    End Enum

End Namespace