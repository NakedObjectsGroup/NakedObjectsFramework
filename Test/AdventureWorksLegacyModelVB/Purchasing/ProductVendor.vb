Namespace AW.Types

	Partial Public Class ProductVendor
		<Hidden>
		Public Property ProductID() As Integer

		<Hidden>
		Public Property VendorID() As Integer

		<MemberOrder(30)>
		Public Property AverageLeadTime() As Integer

		<Mask("C"), MemberOrder(40)>
		Public Property StandardPrice() As Decimal

		<Mask("C"), MemberOrder(41)>
		Public Property LastReceiptCost() As Decimal?

		<Mask("d"), MemberOrder(50)>
		Public Property LastReceiptDate() As DateTime?

		<MemberOrder(60)>
		Public Property MinOrderQty() As Integer

		<MemberOrder(61)>
		Public Property MaxOrderQty() As Integer

		<MemberOrder(62)>
		Public Property OnOrderQty() As Integer?

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		<Hidden>
		Public Property UnitMeasureCode() As String = ""

		<MemberOrder(20)>
		Public Overridable Property UnitMeasure() As UnitMeasure

		<Hidden>
		Public Overridable Property Vendor() As Vendor

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"ProductVendor: {ProductID}-{VendorID}"
		End Function
	End Class
End Namespace