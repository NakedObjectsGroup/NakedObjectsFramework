Namespace AW.Types

	Partial Public Class SalesOrderDetail
		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As SalesOrderDetail)
			SalesOrderID = cloneFrom.SalesOrderID
			SalesOrderDetailID = cloneFrom.SalesOrderDetailID
			OrderQty = cloneFrom.OrderQty
			UnitPrice = cloneFrom.UnitPrice
			UnitPriceDiscount = cloneFrom.UnitPriceDiscount
			LineTotal = cloneFrom.LineTotal
			CarrierTrackingNumber = cloneFrom.CarrierTrackingNumber
			SalesOrderHeader = cloneFrom.SalesOrderHeader
			SpecialOfferID = cloneFrom.SpecialOfferID
			ProductID = cloneFrom.ProductID
			SpecialOfferProduct = cloneFrom.SpecialOfferProduct
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub

		<Hidden>
		Public Property SalesOrderID() As Integer

		<Hidden>
		Public Property SalesOrderDetailID() As Integer

		<MemberOrder(15)>
		Public Property OrderQty() As Short

		<MemberOrder(20), Mask("C")>
		Public Property UnitPrice() As Decimal

		<Named("Discount %"), MemberOrder(30), Mask("P")>
		Public Property UnitPriceDiscount() As Decimal

		<MemberOrder(40), Mask("C")>
		Public Property LineTotal() As Decimal

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? CarrierTrackingNumber {get;set;}
		<MemberOrder(50)>
		Public Property CarrierTrackingNumber() As String

		<Hidden>
		Public Overridable Property SalesOrderHeader() As SalesOrderHeader

		<Hidden>
		Public Property SpecialOfferID() As Integer

		<Hidden>
		Public Property ProductID() As Integer

		<Hidden>
		Public Overridable Property SpecialOfferProduct() As SpecialOfferProduct

		<MemberOrder(11)>
		Public Overridable ReadOnly Property Product() As Product
			Get
				Return SpecialOfferProduct.Product
			End Get
		End Property

		<MemberOrder(12)>
		Public Overridable ReadOnly Property SpecialOffer() As SpecialOffer
			Get
				Return SpecialOfferProduct.SpecialOffer
			End Get
		End Property

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		<Hidden>
		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return $"{OrderQty} x {Product}"
		End Function
	End Class
End Namespace