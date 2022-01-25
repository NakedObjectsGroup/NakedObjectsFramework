Namespace AW.Types

	Partial Public Class ProductVendor

		Implements ITitledObject

		Public Property ProductID() As Integer

		Public Property VendorID() As Integer

#Region "AverageLeadTime"
		Public Property mappedAverageLeadTime As Integer
		Friend myAverageLeadTime As WholeNumber

		<MemberOrder(30)>
		Public ReadOnly Property AverageLeadTime As WholeNumber
			Get
				myAverageLeadTime = If(myAverageLeadTime, New WholeNumber(mappedAverageLeadTime, Sub(v) mappedAverageLeadTime = v))
Return myAverageLeadTime
			End Get
		End Property

		Public Sub AboutAverageLeadTime(a As IFieldAbout, AverageLeadTime As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "StandardPrice"
		Public Property mappedStandardPrice As Decimal
		Friend myStandardPrice As Money

		<MemberOrder(40)>
		Public ReadOnly Property StandardPrice As Money
			Get
				myStandardPrice = If(myStandardPrice, New Money(mappedStandardPrice, Sub(v) mappedStandardPrice = v))
Return myStandardPrice
			End Get
		End Property

		Public Sub AboutStandardPrice(a As IFieldAbout, StandardPrice As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "LastReceiptCost"
		Public Property mappedLastReceiptCost As Decimal?
		Friend myLastReceiptCost As MoneyNullable

		<MemberOrder(41)>
		Public ReadOnly Property LastReceiptCost As MoneyNullable
			Get
				myLastReceiptCost = If(myLastReceiptCost, New MoneyNullable(mappedLastReceiptCost, Sub(v) mappedLastReceiptCost = v))
Return myLastReceiptCost
			End Get
		End Property

		Public Sub AboutLastReceiptCost(a As IFieldAbout, LastReceiptCost As MoneyNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "LastReceiptDate"
		Public Property mappedLastReceiptDate As DateTime?
		Friend myLastReceiptDate As NODateNullable

		<MemberOrder(50)>
		Public ReadOnly Property LastReceiptDate As NODateNullable
			Get
				myLastReceiptDate = If(myLastReceiptDate, New NODateNullable(mappedLastReceiptDate, Sub(v) mappedLastReceiptDate = v))
Return myLastReceiptDate
			End Get
		End Property

		Public Sub AboutLastReceiptDate(a As IFieldAbout, LastReceiptDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "MinOrderQty"
		Public Property mappedMinOrderQty As Integer
		Friend myMinOrderQty As WholeNumber

		<MemberOrder(60)>
		Public ReadOnly Property MinOrderQty As WholeNumber
			Get
				myMinOrderQty = If(myMinOrderQty, New WholeNumber(mappedMinOrderQty, Sub(v) mappedMinOrderQty = v))
Return myMinOrderQty
			End Get
		End Property

		Public Sub AboutMinOrderQty(a As IFieldAbout, MinOrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "MaxOrderQty"
		Public Property mappedMaxOrderQty As Integer
		Friend myMaxOrderQty As WholeNumber

		<MemberOrder(61)>
		Public ReadOnly Property MaxOrderQty As WholeNumber
			Get
				myMaxOrderQty = If(myMaxOrderQty, New WholeNumber(mappedMaxOrderQty, Sub(v) mappedMaxOrderQty = v))
Return myMaxOrderQty
			End Get
		End Property

		Public Sub AboutMaxOrderQty(a As IFieldAbout, MaxOrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "OnOrderQty"
		Public Property mappedOnOrderQty As Integer?
		Friend myOnOrderQty As WholeNumberNullable

		<MemberOrder(62)>
		Public ReadOnly Property OnOrderQty As WholeNumberNullable
			Get
				myOnOrderQty = If(myOnOrderQty, New WholeNumberNullable(mappedOnOrderQty, Sub(v) mappedOnOrderQty = v))
Return myOnOrderQty
			End Get
		End Property

		Public Sub AboutOnOrderQty(a As IFieldAbout, OnOrderQty As WholeNumberNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region
		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		''<Hidden>
		Public Property UnitMeasureCode() As String

		<MemberOrder(20)>
		Public Overridable Property UnitMeasure() As UnitMeasure

		Public Overridable Property Vendor() As Vendor

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

		Public Sub AboutModifiedDate(a As IFieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"ProductVendor: {ProductID}-{VendorID}"
		End Function
	End Class
End Namespace