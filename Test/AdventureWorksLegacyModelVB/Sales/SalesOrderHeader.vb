Namespace AW.Types

	<Named("Sales Order")>
	Partial Public Class SalesOrderHeader

		'<Hidden>
		Public Property SalesOrderID() As Integer

#Region "SalesOrderNumber"
		Friend mappedSalesOrderNumber As String
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
		Friend mappedAddItemsFromCart As Boolean
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

		'<Hidden>
		Public Property StatusByte() As Byte

		<MemberOrder(1)>
		Public Overridable ReadOnly Property Status() As TextString
			Get
				Return New TextString([Enum].GetName(GetType(OrderStatus), StatusByte))
			End Get
		End Property

		'<Hidden>
		Public Property CustomerID() As Integer

		<MemberOrder(2)>
		Public Overridable Property Customer() As Customer

		'<Hidden>
		Public Property BillingAddressID() As Integer

		<MemberOrder(4)>
		Public Overridable Property BillingAddress() As Address

#Region "PurchaseOrderNumber"
		Friend mappedPurchaseOrderNumber As String
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

		'<Hidden>
		Public Property ShippingAddressID() As Integer

		<MemberOrder(10)>
		Public Overridable Property ShippingAddress() As Address

		'<Hidden>
		Public Property ShipMethodID() As Integer

		<MemberOrder(11)>
		Public Overridable Property ShipMethod() As ShipMethod

#Region "AccountNumber"
		Friend mappedAccountNumber As String
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
		Friend mappedOrderDate As Date
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
		Friend mappedDueDate As Date
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
		Friend mappedShipDate As Date?
		Friend myShipDate As NODate

		<MemberOrder(22)>
		Public ReadOnly Property ShipDate As NODate
			Get
				Return If(myShipDate, New NODate(mappedShipDate, Function(v) mappedShipDate = v))
			End Get
		End Property

		Public Sub AboutShipDate(a As FieldAbout, ShipDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "SubTotal"
		Friend mappedSubTotal As Decimal
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
		Friend mappedTaxAmt As Decimal
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
		Friend mappedFreight As Decimal
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
		Friend mappedTotalDue As Decimal
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

		'<Hidden>
		Public Property CurrencyRateID() As Integer?

		<MemberOrder(35)>
		Public Overridable Property CurrencyRate() As CurrencyRate

#Region "OnlineOrder"
		Friend mappedOnlineOrder As Boolean
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