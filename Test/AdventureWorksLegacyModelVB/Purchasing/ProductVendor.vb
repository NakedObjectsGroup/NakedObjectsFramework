Namespace AW.Types

	Partial Public Class ProductVendor

		Implements ITitledObject

		Public Property ProductID() As Integer

		Public Property VendorID() As Integer

#Region "AverageLeadTime"
		Public mappedAverageLeadTime As Integer
		Friend myAverageLeadTime As WholeNumber

		'<MemberOrder(30)>
		Public ReadOnly Property AverageLeadTime As WholeNumber
			Get
				Return If(myAverageLeadTime, New WholeNumber(mappedAverageLeadTime, Function(v) mappedAverageLeadTime = v))
			End Get
		End Property

		Public Sub AboutAverageLeadTime(a As FieldAbout, AverageLeadTime As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "StandardPrice"
		Public mappedStandardPrice As Decimal
		Friend myStandardPrice As Money

		'<MemberOrder(40)>
		Public ReadOnly Property StandardPrice As Money
			Get
				Return If(myStandardPrice, New Money(mappedStandardPrice, Function(v) mappedStandardPrice = v))
			End Get
		End Property

		Public Sub AboutStandardPrice(a As FieldAbout, StandardPrice As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "LastReceiptCost"
		Public mappedLastReceiptCost As Decimal?
		Friend myLastReceiptCost As MoneyNullable

		'<MemberOrder(41)>
		Public ReadOnly Property LastReceiptCost As MoneyNullable
			Get
				Return If(myLastReceiptCost, New MoneyNullable(mappedLastReceiptCost, Function(v) mappedLastReceiptCost = v))
			End Get
		End Property

		Public Sub AboutLastReceiptCost(a As FieldAbout, LastReceiptCost As MoneyNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "LastReceiptDate"
		Public mappedLastReceiptDate As DateTime?
		Friend myLastReceiptDate As NODateNullable

		'<MemberOrder(50)>
		Public ReadOnly Property LastReceiptDate As NODateNullable
			Get
				Return If(myLastReceiptDate, New NODateNullable(mappedLastReceiptDate, Function(v) mappedLastReceiptDate = v))
			End Get
		End Property

		Public Sub AboutLastReceiptDate(a As FieldAbout, LastReceiptDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "MinOrderQty"
		Public mappedMinOrderQty As Integer
		Friend myMinOrderQty As WholeNumber

		'<MemberOrder(60)>
		Public ReadOnly Property MinOrderQty As WholeNumber
			Get
				Return If(myMinOrderQty, New WholeNumber(mappedMinOrderQty, Function(v) mappedMinOrderQty = v))
			End Get
		End Property

		Public Sub AboutMinOrderQty(a As FieldAbout, MinOrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "MaxOrderQty"
		Public mappedMaxOrderQty As Integer
		Friend myMaxOrderQty As WholeNumber

		'<MemberOrder(61)>
		Public ReadOnly Property MaxOrderQty As WholeNumber
			Get
				Return If(myMaxOrderQty, New WholeNumber(mappedMaxOrderQty, Function(v) mappedMaxOrderQty = v))
			End Get
		End Property

		Public Sub AboutMaxOrderQty(a As FieldAbout, MaxOrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "OnOrderQty"
		Public mappedOnOrderQty As Integer?
		Friend myOnOrderQty As WholeNumberNullable

		'<MemberOrder(62)>
		Public ReadOnly Property OnOrderQty As WholeNumberNullable
			Get
				Return If(myOnOrderQty, New WholeNumberNullable(mappedOnOrderQty, Function(v) mappedOnOrderQty = v))
			End Get
		End Property

		Public Sub AboutOnOrderQty(a As FieldAbout, OnOrderQty As WholeNumberNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region
		'<MemberOrder(10)>
		Public Overridable Property Product() As Product

		''<Hidden>
		Public Property UnitMeasureCode() As String

		'<MemberOrder(20)>
		Public Overridable Property UnitMeasure() As UnitMeasure

		Public Overridable Property Vendor() As Vendor

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

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"ProductVendor: {ProductID}-{VendorID}"
		End Function
	End Class
End Namespace