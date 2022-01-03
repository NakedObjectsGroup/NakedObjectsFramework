Namespace AW.Types

	Partial Public Class PurchaseOrderHeader

		''<Hidden>
		Public Property PurchaseOrderID() As Integer

#Region "RevisionNumber"
		Public mappedRevisionNumber As Byte
		Friend myRevisionNumber As WholeNumber

		'<MemberOrder(90)>
		Private ReadOnly Property RevisionNumber As WholeNumber
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

		''<Hidden>
		Public Property ShipMethodID() As Integer

		'<MemberOrder(22)>
		Public Overridable Property ShipMethod() As ShipMethod

#Region "Details (Collection)"
		Public Overridable Property mappedDetails As ICollection(Of PurchaseOrderDetail) = New List(Of PurchaseOrderDetail)()

		Private myDetails As InternalCollection

		'''<TableView(True, "OrderQty", "Product", "UnitPrice", "LineTotal")>
		'<MemberOrder(1)>
		Private ReadOnly Property Details As InternalCollection
			Get
				Return If(myDetails, New InternalCollection(Of PurchaseOrderDetail)(mappedDetails))
			End Get
		End Property

		Public Sub AboutDetails(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property VendorID() As Integer

		'<MemberOrder(1)>
		Public Overridable Property Vendor() As Vendor

		''<Hidden>
		Public Property Status() As Byte

		Private ReadOnly Property StatusAsEnum As TextString
			Get
				Return New TextString([Enum].GetName(GetType(POStatus), Status))
			End Get
		End Property

		Public Sub AboutStatusAsEnum(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Status"
			End Select
		End Sub

#Region "OrderDate"
		Public mappedOrderDate As DateTime
		Friend myOrderDate As NODate

		'<MemberOrder(11)>
		Private ReadOnly Property OrderDate As TextString
			Get
				Return If(myOrderDate, New TextString(mappedOrderDate, Function(v) mappedOrderDate = v))
			End Get
		End Property

		Public Sub AboutOrderDate(a As FieldAbout, OrderDate As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ShipDate"
		Public mappedShipDate As DateTime?
		Friend myShipDate As NODate

		'<MemberOrder(20)>
		Private ReadOnly Property ShipDate As NODate
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
		Public mappedSubTotal As Decimal
		Friend mySubTotal As Money

		'<MemberOrder(31)>
		Private ReadOnly Property SubTotal As Money
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
		Public mappedTaxAmt As Decimal
		Friend myTaxAmt As Money

		'<MemberOrder(32)>
		Private ReadOnly Property TaxAmt As Money
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
		Public mappedFreight As Decimal
		Friend myFreight As Money

		'<MemberOrder(33)>
		Private ReadOnly Property Freight As Money
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
		Public mappedTotalDue As Decimal
		Friend myTotalDue As Money

		'<MemberOrder(34)>
		Private ReadOnly Property TotalDue As Money
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
		Public Property OrderPlacedByID() As Integer

		'<MemberOrder(12)>
		Public Overridable Property OrderPlacedBy() As Employee

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
		Private ReadOnly Property ModifiedDate As TimeStamp
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

		Public Function Title() As Title
			Return New Title($"PO from {Vendor}, {OrderDate}")
		End Function

	End Class

	Public Enum POStatus
		Pending = 1
		Approved = 2
		Rejected = 3
		Complete = 4
	End Enum
End Namespace