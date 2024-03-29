﻿Namespace AW.Types

	Partial Public Class SalesOrderDetail

		Implements ITitledObject, INotEditableOncePersistent

		'<Hidden>
		Public Property SalesOrderID() As Integer

		'<Hidden>
		Public Property SalesOrderDetailID() As Integer

#Region "OrderQty"
		Public Property mappedOrderQty As Short
		Friend myOrderQty As WholeNumber

		<DemoProperty(Order:=15)>
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

		<DemoProperty(Order:=20)>
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

#Region "UnitPriceDiscount"
		Public Property mappedUnitPriceDiscount As Decimal
		Friend myUnitPriceDiscount As Percentage

		<DemoProperty(Order:=30)>
		Public ReadOnly Property UnitPriceDiscount As Percentage
			Get
				myUnitPriceDiscount = If(myUnitPriceDiscount, New Percentage(mappedUnitPriceDiscount, Sub(v) mappedUnitPriceDiscount = v))
				Return myUnitPriceDiscount
			End Get
		End Property

		Public Sub AboutUnitPriceDiscount(a As FieldAbout, UnitPriceDiscount As Percentage)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Discount %"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "LineTotal"
		Public Property mappedLineTotal As Decimal
		Friend myLineTotal As Money

		<DemoProperty(Order:=40)>
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

#Region "CarrierTrackingNumber"
		Public Property mappedCarrierTrackingNumber As String
		Friend myCarrierTrackingNumber As TextString

		<DemoProperty(Order:=50)>
		Public ReadOnly Property CarrierTrackingNumber As TextString
			Get
				myCarrierTrackingNumber = If(myCarrierTrackingNumber, New TextString(mappedCarrierTrackingNumber, Sub(v) mappedCarrierTrackingNumber = v))
				Return myCarrierTrackingNumber
			End Get
		End Property

		Public Sub AboutCarrierTrackingNumber(a As FieldAbout, CarrierTrackingNumber As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		'<Hidden>
		Public Overridable Property SalesOrderHeader() As SalesOrderHeader

		'<Hidden>
		Public Property SpecialOfferID() As Integer

		'<Hidden>
		Public Property ProductID() As Integer

		'<Hidden>
		Public Overridable Property SpecialOfferProduct() As SpecialOfferProduct

		<DemoProperty(Order:=11)>
		Public Overridable ReadOnly Property Product() As Product
			Get
				Return SpecialOfferProduct.Product
			End Get
		End Property

		<DemoProperty(Order:=12)>
		Public Overridable ReadOnly Property SpecialOffer() As SpecialOffer
			Get
				Return SpecialOfferProduct.SpecialOffer
			End Get
		End Property


#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<DemoProperty(Order:=99)>
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
		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{OrderQty} x {Product}"
		End Function
	End Class
End Namespace