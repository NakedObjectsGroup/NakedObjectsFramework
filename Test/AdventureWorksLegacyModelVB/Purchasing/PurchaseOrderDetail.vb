Namespace AW.Types

	Partial Public Class PurchaseOrderDetail

		Implements ITitledObject, INotEditableOncePersistent

		''<Hidden>
		Public Property PurchaseOrderID() As Integer

		''<Hidden>
		Public Property PurchaseOrderDetailID() As Integer

#Region "DueDate"
		Public Property mappedDueDate As Date
		Friend myDueDate As NODate

		<MemberOrder(26)>
		Public ReadOnly Property DueDate As NODate
			Get
				myDueDate = If(myDueDate, New NODate(mappedDueDate, Sub(v) mappedDueDate = v))
				Return myDueDate
			End Get
		End Property

		Public Sub AboutDueDate(a As FieldAbout, DueDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "OrderQty"
		Public Property mappedOrderQty As Short
		Friend myOrderQty As WholeNumber

		<MemberOrder(20)>
		Public ReadOnly Property OrderQty As WholeNumber
			Get
				myOrderQty = If(myOrderQty, New WholeNumber(mappedOrderQty, Sub(v) mappedOrderQty = CType(v, Short)))
				Return myOrderQty
			End Get
		End Property

		Public Sub AboutOrderQty(a As FieldAbout, OrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "UnitPrice"
		Public Property mappedUnitPrice As Decimal
		Friend myUnitPrice As Money

		<MemberOrder(22)>
		Public ReadOnly Property UnitPrice As Money
			Get
				myUnitPrice = If(myUnitPrice, New Money(mappedUnitPrice, Sub(v) mappedUnitPrice = v))
				Return myUnitPrice
			End Get
		End Property

		Public Sub AboutUnitPrice(a As FieldAbout, UnitPrice As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "LineTotal"
		Public Property mappedLineTotal As Decimal
		Friend myLineTotal As Money

		<MemberOrder(24)>
		Public ReadOnly Property LineTotal As Money
			Get
				myLineTotal = If(myLineTotal, New Money(mappedLineTotal, Sub(v) mappedLineTotal = v))
				Return myLineTotal
			End Get
		End Property

		Public Sub AboutLineTotal(a As FieldAbout, LineTotal As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ReceivedQty"
		Public Property mappedReceivedQty As Decimal
		Friend myReceivedQty As FloatingPointNumber

		<MemberOrder(30)>
		Public ReadOnly Property ReceivedQty As FloatingPointNumber
			Get
				myReceivedQty = If(myReceivedQty, New FloatingPointNumber(mappedReceivedQty, Sub(v) mappedReceivedQty = v))
				Return myReceivedQty
			End Get
		End Property

		Public Sub AboutReceivedQty(a As FieldAbout, ReceivedQty As FloatingPointNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region
#Region "RejectedQty"
		Public Property mappedRejectedQty As Decimal
		Friend myRejectedQty As FloatingPointNumber

		<MemberOrder(32)>
		Public ReadOnly Property RejectedQty As FloatingPointNumber
			Get
				myRejectedQty = If(myRejectedQty, New FloatingPointNumber(mappedRejectedQty, Sub(v) mappedRejectedQty = v))
				Return myRejectedQty
			End Get
		End Property

		Public Sub AboutRejectedQty(a As FieldAbout, RejectedQty As FloatingPointNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region
#Region "StockedQty"
		Public Property mappedStockedQty As Decimal
		Friend myStockedQty As FloatingPointNumber

		<MemberOrder(34)>
		Public ReadOnly Property StockedQty As FloatingPointNumber
			Get
				myStockedQty = If(myStockedQty, New FloatingPointNumber(mappedStockedQty, Sub(v) mappedStockedQty = v))
				Return myStockedQty
			End Get
		End Property

		Public Sub AboutStockedQty(a As FieldAbout, StockedQty As FloatingPointNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
				Return myModifiedDate
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		Public Overridable Property PurchaseOrderHeader() As PurchaseOrderHeader

		Public Sub AboutPurchaseOrderHeader(a As FieldAbout)
			Select Case a.TypeCode
				Case Else
					a.Visible = False
			End Select
		End Sub

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{OrderQty} x {Product}"
		End Function
	End Class
End Namespace