Namespace AW.Types

	Partial Public Class PurchaseOrderDetail

		<Hidden>
		Public Property PurchaseOrderID() As Integer

		<Hidden>
		Public Property PurchaseOrderDetailID() As Integer

		<MemberOrder(26), Mask("d")>
		Public Property DueDate() As DateTime

		<MemberOrder(20)>
		Public Property OrderQty() As Short

		<MemberOrder(22), Mask("C")>
		Public Property UnitPrice() As Decimal

		<MemberOrder(24), Mask("C")>
		Public Property LineTotal() As Decimal

		<MemberOrder(30), Mask("#")>
		Public Property ReceivedQty() As Decimal

		<MemberOrder(32), Mask("#")>
		Public Property RejectedQty() As Decimal

		<MemberOrder(34), Mask("#")>
		Public Property StockedQty() As Decimal

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		<Hidden>
		Public Overridable Property PurchaseOrderHeader() As PurchaseOrderHeader

		Public Overrides Function ToString() As String
			Return $"{OrderQty} x {Product}"
		End Function
	End Class
End Namespace