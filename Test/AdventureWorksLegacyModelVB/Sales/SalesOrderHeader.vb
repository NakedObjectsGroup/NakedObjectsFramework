Namespace AW.Types

	'<Named("Sales Order")>
	Partial Public Class SalesOrderHeader

		Implements ITitledObject

		''<Hidden>
		Public Property SalesOrderID() As Integer

#Region "SalesOrderNumber"
		Public Property mappedSalesOrderNumber As String
		Friend mySalesOrderNumber As TextString

		<MemberOrder(1)>
		Public ReadOnly Property SalesOrderNumber As TextString
			Get
				Return If(mySalesOrderNumber, New TextString(mappedSalesOrderNumber, Function(v) mappedSalesOrderNumber = v))
			End Get
		End Property

		Public Sub AboutSalesOrderNumber(a As FieldAbout, SalesOrderNumber As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "AddItemsFromCart"
		Public Property mappedAddItemsFromCart As Boolean
		Friend myAddItemsFromCart As Logical

		<MemberOrder(1)>
		Public ReadOnly Property AddItemsFromCart As Logical
			Get
				Return If(myAddItemsFromCart, New Logical(mappedAddItemsFromCart, Function(v) mappedAddItemsFromCart = v))
			End Get
		End Property

		Public Sub AboutAddItemsFromCart(a As FieldAbout, AddItemsFromCart As Logical)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Status"
		Public Property Status() As Byte

		<MemberOrder(1)>
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

		<MemberOrder(2)>
		Public Overridable Property Customer() As Customer
#End Region

#Region "BillingAddress"
		''<Hidden>
		Public Property BillingAddressID() As Integer

		<MemberOrder(4)>
		Public Overridable Property BillingAddress() As Address
#End Region
#Region "PurchaseOrderNumber"
		Public Property mappedPurchaseOrderNumber As String
		Friend myPurchaseOrderNumber As TextString

		<MemberOrder(5)>
		Public ReadOnly Property PurchaseOrderNumber As TextString
			Get
				Return If(myPurchaseOrderNumber, New TextString(mappedPurchaseOrderNumber, Function(v) mappedPurchaseOrderNumber = v))
			End Get
		End Property

		Public Sub AboutPurchaseOrderNumber(a As FieldAbout, PurchaseOrderNumber As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property ShippingAddressID() As Integer

		<MemberOrder(10)>
		Public Overridable Property ShippingAddress() As Address

		''<Hidden>
		Public Property ShipMethodID() As Integer

		<MemberOrder(11)>
		Public Overridable Property ShipMethod() As ShipMethod

#Region "AccountNumber"
		Public Property mappedAccountNumber As String
		Friend myAccountNumber As TextString

		<MemberOrder(12)>
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

#Region "OrderDate"
		Public Property mappedOrderDate As Date
		Friend myOrderDate As NODate

		<MemberOrder(20)>
		Public ReadOnly Property OrderDate As NODate
			Get
				Return If(myOrderDate, New NODate(mappedOrderDate, Function(v) mappedOrderDate = v))
			End Get
		End Property

		Public Sub AboutOrderDate(a As FieldAbout, OrderDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "DueDate"
		Public Property mappedDueDate As Date
		Friend myDueDate As NODate

		<MemberOrder(21)>
		Public ReadOnly Property DueDate As NODate
			Get
				Return If(myDueDate, New NODate(mappedDueDate, Function(v) mappedDueDate = v))
			End Get
		End Property

		Public Sub AboutDueDate(a As FieldAbout, DueDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ShipDate"
		Public Property mappedShipDate As Date?
		Friend myShipDate As NODateNullable

		<MemberOrder(22)>
		Public ReadOnly Property ShipDate As NODateNullable
			Get
				Return If(myShipDate, New NODateNullable(mappedShipDate, Function(v) mappedShipDate = v))
			End Get
		End Property

		Public Sub AboutShipDate(a As FieldAbout, ShipDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "SubTotal"
		Public Property mappedSubTotal As Decimal
		Friend mySubTotal As Money

		<MemberOrder(31)>
		Public ReadOnly Property SubTotal As Money
			Get
				Return If(mySubTotal, New Money(mappedSubTotal, Function(v) mappedSubTotal = v))
			End Get
		End Property

		Public Sub AboutSubTotal(a As FieldAbout, SubTotal As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "TaxAmt"
		Public Property mappedTaxAmt As Decimal
		Friend myTaxAmt As Money

		<MemberOrder(32)>
		Public ReadOnly Property TaxAmt As Money
			Get
				Return If(myTaxAmt, New Money(mappedTaxAmt, Function(v) mappedTaxAmt = v))
			End Get
		End Property

		Public Sub AboutTaxAmt(a As FieldAbout, TaxAmt As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Freight"
		Public Property mappedFreight As Decimal
		Friend myFreight As Money

		<MemberOrder(33)>
		Public ReadOnly Property Freight As Money
			Get
				Return If(myFreight, New Money(mappedFreight, Function(v) mappedFreight = v))
			End Get
		End Property

		Public Sub AboutFreight(a As FieldAbout, Freight As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "TotalDue"
		Public Property mappedTotalDue As Decimal
		Friend myTotalDue As Money

		<MemberOrder(34)>
		Public ReadOnly Property TotalDue As Money
			Get
				Return If(myTotalDue, New Money(mappedTotalDue, Function(v) mappedTotalDue = v))
			End Get
		End Property

		Public Sub AboutTotalDue(a As FieldAbout, TotalDue As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property CurrencyRateID() As Integer?

		<MemberOrder(35)>
		Public Overridable Property CurrencyRate() As CurrencyRate

#Region "OnlineOrder"
		Public Property mappedOnlineOrder As Boolean
		Friend myOnlineOrder As Logical

		<MemberOrder(41)>
		Public ReadOnly Property OnlineOrder As Logical
			Get
				Return If(myOnlineOrder, New Logical(mappedOnlineOrder, Function(v) mappedOnlineOrder = v))
			End Get
		End Property

		Public Sub AboutOnlineOrder(a As FieldAbout, OnlineOrder As Logical)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Online Order"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "CreditCard"

		Public Property CreditCardID As Integer?

		<MemberOrder(42)>
		Public Overridable Property CreditCard As CreditCard

#End Region

#Region "CreditCardApprovalCoad"
		Public Property mappedCreditCardApprovalCode As String
		Friend myCreditCardApprovalCode As TextString

		<MemberOrder(43)>
		Public ReadOnly Property CreditCardApprovalCode As TextString
			Get
				Return If(myCreditCardApprovalCode, New TextString(mappedCreditCardApprovalCode, Function(v) mappedCreditCardApprovalCode = v))
			End Get
		End Property

		Public Sub AboutCreditCardApprovalCode(a As FieldAbout, CreditCardApprovalCode As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
					a.Usable = False
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "RevisionNumber"
		Public Property mappedRevisionNumber As Byte
		Friend myRevisionNumber As WholeNumber

		<MemberOrder(51)>
		Public ReadOnly Property RevisionNumber As WholeNumber
			Get
				Return If(myRevisionNumber, New WholeNumber(mappedRevisionNumber, Function(v) mappedRevisionNumber = v))
			End Get
		End Property

		Public Sub AboutRevisionNumber(a As FieldAbout, RevisionNumber As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Comment"
		Public Property mappedComment As String
		Friend myComment As TextString
		'[MultiLine(NumberOfLines = 3, Width = 50)]
		<MemberOrder(52)>
		Public ReadOnly Property Comment As TextString
			Get
				Return If(myComment, New TextString(mappedComment, Function(v) mappedComment = v))
			End Get
		End Property

		Public Sub AboutComment(a As FieldAbout, Comment As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Description = "Free-form text"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
					'comment is not null or empty

			End Select
		End Sub
#End Region

#Region "SalesPerson"

		Public Property SalesPersonID As Integer?

		<MemberOrder(62)>
		Public Overridable Property SalesPerson As SalesTerritory

#End Region

#Region "SalesTerritory"

		Public Property SalesTerritoryID As Integer?

		<MemberOrder(62)>
		Public Overridable Property SalesTerritory As SalesTerritory

#End Region

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
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

		<MemberOrder(1)>
		Public ReadOnly Property Details As InternalCollection
			Get
				Return If(myDetails, New InternalCollection(Of SalesOrderDetail)(mappedDetails))
			End Get
		End Property

		Public Sub AboutDetails(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Reasons (Collection)"
		Public Overridable Property mappedReasons As ICollection(Of SalesOrderHeaderSalesReason) = New List(Of SalesOrderHeaderSalesReason)()

		Private myReasons As InternalCollection

		<MemberOrder(1)>
		Public ReadOnly Property Reasons As InternalCollection
			Get
				Return If(myReasons, New InternalCollection(Of SalesOrderHeaderSalesReason)(mappedReasons))
			End Get
		End Property

		Public Sub AboutReasons(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property RowGuid() As Guid

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedSalesOrderNumber
		End Function

#Region "Actions"
		Public Sub ActionClearComments()
			Throw New NotImplementedException
		End Sub

		Public Sub ActionAppendComment()
			Throw New NotImplementedException
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