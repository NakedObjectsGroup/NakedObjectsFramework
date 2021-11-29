Namespace AW.Types

	Partial Public Class PurchaseOrderHeader
		Implements IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As PurchaseOrderHeader)
			PurchaseOrderID = cloneFrom.PurchaseOrderID
			RevisionNumber = cloneFrom.RevisionNumber
			ShipMethodID = cloneFrom.ShipMethodID
			ShipMethod = cloneFrom.ShipMethod
			Details = cloneFrom.Details
			VendorID = cloneFrom.VendorID
			Vendor = cloneFrom.Vendor
			Status = cloneFrom.Status
			OrderDate = cloneFrom.OrderDate
			ShipDate = cloneFrom.ShipDate
			SubTotal = cloneFrom.SubTotal
			TaxAmt = cloneFrom.TaxAmt
			Freight = cloneFrom.Freight
			TotalDue = cloneFrom.TotalDue
			OrderPlacedByID = cloneFrom.OrderPlacedByID
			OrderPlacedBy = cloneFrom.OrderPlacedBy
			ModifiedDate = cloneFrom.ModifiedDate
		End Sub

		<Hidden>
		Public Property PurchaseOrderID() As Integer

		<MemberOrder(90)>
		Public Property RevisionNumber() As Byte

		<Hidden>
		Public Property ShipMethodID() As Integer

		<MemberOrder(22)>
		Public Overridable Property ShipMethod() As ShipMethod

		<TableView(True, "OrderQty", "Product", "UnitPrice", "LineTotal")>
		Public Overridable Property Details() As ICollection(Of PurchaseOrderDetail)

		<Hidden>
		Public Property VendorID() As Integer

		<MemberOrder(1)>
		Public Overridable Property Vendor() As Vendor

		<Hidden>
		Public Property Status() As Byte

		<Named("Status"), MemberOrder(1)>
		Public Overridable ReadOnly Property StatusAsEnum() As POStatus
			Get
				Return CType(Status, POStatus)
			End Get
		End Property

		<MemberOrder(11), Mask("d")>
		Public Property OrderDate() As DateTime

		<MemberOrder(20), Mask("d")>
		Public Property ShipDate() As DateTime?

		<MemberOrder(31), Mask("C")>
		Public Property SubTotal() As Decimal

		<MemberOrder(32), Mask("C")>
		Public Property TaxAmt() As Decimal

		<MemberOrder(33), Mask("C")>
		Public Property Freight() As Decimal

		<MemberOrder(34), Mask("C")>
		Public Property TotalDue() As Decimal

		<Hidden>
		Public Property OrderPlacedByID() As Integer

		<MemberOrder(12)>
		Public Overridable Property OrderPlacedBy() As Employee

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"PO from {Vendor}, {OrderDate}"
		End Function
	End Class

	Public Enum POStatus
		Pending = 1
		Approved = 2
		Rejected = 3
		Complete = 4
	End Enum
End Namespace