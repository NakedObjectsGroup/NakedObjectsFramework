Namespace AW.Types

	Partial Public Class SalesOrderDetail

		'<Hidden>
		Public Property SalesOrderID() As Integer

		'<Hidden>
		Public Property SalesOrderDetailID() As Integer

#Region "OrderQty"
		Public mappedOrderQty As Short
		Friend myOrderQty As WholeNumber

		'<MemberOrder(15)>
		Private ReadOnly Property OrderQty As WholeNumber
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

		'<MemberOrder(20)>
		Private ReadOnly Property UnitPrice As Money
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

		'TODO: Add Percentage valueholder
		'<Named("Discount %"), MemberOrder(30), Mask("P")>
		Public Property UnitPriceDiscount() As Decimal

#Region "LineTotal"
		Public mappedLineTotal As Decimal
		Friend myLineTotal As Money

		'<MemberOrder(40)>
		Private ReadOnly Property LineTotal As Money
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

#Region "CarrierTrackingNumber"
		Public mappedCarrierTrackingNumber As String
		Friend myCarrierTrackingNumber As TextString

		'<MemberOrder(50)>
		Private ReadOnly Property CarrierTrackingNumber As TextString
			Get
				Return If(myCarrierTrackingNumber, New TextString(mappedCarrierTrackingNumber, Function(v) mappedCarrierTrackingNumber = v))
			End Get
		End Property

		Public Sub AboutCarrierTrackingNumber(a As FieldAbout, CarrierTrackingNumber As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
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

		'<MemberOrder(11)>
		Public Overridable ReadOnly Property Product() As Product
			Get
				Return SpecialOfferProduct.Product
			End Get
		End Property

		'<MemberOrder(12)>
		Public Overridable ReadOnly Property SpecialOffer() As SpecialOffer
			Get
				Return SpecialOfferProduct.SpecialOffer
			End Get
		End Property


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

		''<Hidden>
		Public Property RowGuid() As Guid

		Public Function Title() As Title
			Return New Title($"{OrderQty} x {Product}")
		End Function
	End Class
End Namespace