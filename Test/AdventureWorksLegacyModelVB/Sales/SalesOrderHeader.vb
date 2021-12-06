Namespace AW.Types

	<Named("Sales Order")>
	Partial Public Class SalesOrderHeader

		<Hidden>
		Public Property SalesOrderID() As Integer

		<MemberOrder(1)>
		Public Property SalesOrderNumber() As String = ""

		Public Property AddItemsFromCart() As Boolean

		<Hidden>
		Public Property StatusByte() As Byte

		<MemberOrder(1)>
		Public Overridable ReadOnly Property Status() As OrderStatus
			Get
				Return CType(StatusByte, OrderStatus)
			End Get
		End Property

		<Hidden>
		Public Property CustomerID() As Integer

		<MemberOrder(2)>
		Public Overridable Property Customer() As Customer

		<Hidden>
		Public Property BillingAddressID() As Integer

		<MemberOrder(4)>
		Public Overridable Property BillingAddress() As Address

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? PurchaseOrderNumber {get;set;}
		<MemberOrder(5)>
		Public Property PurchaseOrderNumber() As String

		<Hidden>
		Public Property ShippingAddressID() As Integer

		<MemberOrder(10)>
		Public Overridable Property ShippingAddress() As Address

		<Hidden>
		Public Property ShipMethodID() As Integer

		<MemberOrder(11)>
		Public Overridable Property ShipMethod() As ShipMethod

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? AccountNumber {get;set;}
		<MemberOrder(12)>
		Public Property AccountNumber() As String

		<MemberOrder(20)>
		Public Property OrderDate() As DateTime

		<MemberOrder(21)>
		Public Property DueDate() As DateTime

		<MemberOrder(22), Mask("d")>
		Public Property ShipDate() As DateTime?

		<MemberOrder(31), Mask("C")>
		Public Property SubTotal() As Decimal

		<MemberOrder(32), Mask("C")>
		Public Property TaxAmt() As Decimal

		<MemberOrder(33), Mask("C")>
		Public Property Freight() As Decimal

		<MemberOrder(34)>
		<Mask("C")>
		Public Property TotalDue() As Decimal

		<Hidden>
		Public Property CurrencyRateID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual CurrencyRate? CurrencyRate {get;set;}
		<MemberOrder(35)>
		Public Overridable Property CurrencyRate() As CurrencyRate

		<DescribedAs("Order has been placed via the web")>
		<MemberOrder(41), Named("Online Order")>
		Public Property OnlineOrder() As Boolean

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