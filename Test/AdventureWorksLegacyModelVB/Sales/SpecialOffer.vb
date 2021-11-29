Namespace AW.Types

	Partial Public Class SpecialOffer
		Implements IHasModifiedDate, IHasRowGuid

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As SpecialOffer)
			SpecialOfferID = cloneFrom.SpecialOfferID
			Description = cloneFrom.Description
			DiscountPct = cloneFrom.DiscountPct
			Type = cloneFrom.Type
			Category = cloneFrom.Category
			StartDate = cloneFrom.StartDate
			EndDate = cloneFrom.EndDate
			MinQty = cloneFrom.MinQty
			MaxQty = cloneFrom.MaxQty
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub
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

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Description
		End Function
	End Class
End Namespace