Namespace AW.Types

	Partial Public Class PurchaseOrderHeader
		

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

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
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