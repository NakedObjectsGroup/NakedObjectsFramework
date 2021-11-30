Namespace AW.Types

	Partial Public Class SpecialOffer
		Implements IHasModifiedDate, IHasRowGuid

		<Hidden>
		Public Property SpecialOfferID() As Integer

		<MemberOrder(10)>
		Public Property Description() As String = ""

		<MemberOrder(20)>
		<Mask("P")>
		Public Property DiscountPct() As Decimal

		<MemberOrder(30)>
		Public Property Type() As String = ""

		<MemberOrder(40)>
		Public Property Category() As String = ""

		<MemberOrder(51)>
		<Mask("d")>
		Public Property StartDate() As DateTime

		<MemberOrder(52)>
		<Mask("d")>
		Public Property EndDate() As DateTime

		<MemberOrder(61)>
		Public Property MinQty() As Integer

		<MemberOrder(62)>
		Public Property MaxQty() As Integer?

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
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

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Description
		End Function
	End Class
End Namespace