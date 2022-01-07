Namespace AW.Types

	Partial Public Class PurchaseOrderDetail
 Implements ITitledObject

		''<Hidden>
		Public Property PurchaseOrderID() As Integer

		''<Hidden>
		Public Property PurchaseOrderDetailID() As Integer

#Region "DueDate"
		Public mappedDueDate As Date
		Friend myDueDate As NODate

		'<MemberOrder(26)>
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

#Region "OrderQty"
		Public mappedOrderQty As Short
		Friend myOrderQty As WholeNumber

		'<MemberOrder(20)>
		Public ReadOnly Property OrderQty As WholeNumber
			Get
				Return If(myOrderQty, New WholeNumber(mappedOrderQty, Function(v) mappedOrderQty = v))
			End Get
		End Property

		Public Sub AboutOrderQty(a As FieldAbout, OrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "UnitPrice"
		Public mappedUnitPrice As Decimal
		Friend myUnitPrice As Money

		'<MemberOrder(22)>
		Public ReadOnly Property UnitPrice As Money
			Get
				Return If(myUnitPrice, New Money(mappedUnitPrice, Function(v) mappedUnitPrice = v))
			End Get
		End Property

		Public Sub AboutUnitPrice(a As FieldAbout, UnitPrice As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "LineTotal"
		Public mappedLineTotal As Decimal
		Friend myLineTotal As Money

		'<MemberOrder(24)>
		Public ReadOnly Property LineTotal As Money
			Get
				Return If(myLineTotal, New Money(mappedLineTotal, Function(v) mappedLineTotal = v))
			End Get
		End Property

		Public Sub AboutLineTotal(a As FieldAbout, LineTotal As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		'TODO: Need a new decimal valueholder - not Money - 3 fields
#Region "ReceivedQty"
		Public mappedReceivedQty As Decimal
		Friend myReceivedQty As Money

		'<MemberOrder(30)>
		Public ReadOnly Property ReceivedQty As Money
			Get
				Return If(myReceivedQty, New Money(mappedReceivedQty, Function(v) mappedReceivedQty = v))
			End Get
		End Property

		Public Sub AboutReceivedQty(a As FieldAbout, ReceivedQty As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		''<MemberOrder(32), Mask("#")>
		Public Property RejectedQty() As Decimal

		''<MemberOrder(34), Mask("#")>
		Public Property StockedQty() As Decimal

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
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

		''<Hidden>
		Public Property ProductID() As Integer

		'<MemberOrder(10)>
		Public Overridable Property Product() As Product

		Public Overridable Property PurchaseOrderHeader() As PurchaseOrderHeader

		Public Sub AboutPurchaseOrderHeader(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{OrderQty} x {Product}"
		End Function
	End Class
End Namespace