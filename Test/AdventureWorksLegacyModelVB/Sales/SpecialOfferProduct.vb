Namespace AW.Types

	Partial Public Class SpecialOfferProduct
		<Hidden>
		Public Property SpecialOfferID() As Integer


		<MemberOrder(1)>
		Public Overridable Property SpecialOffer() As SpecialOffer


		<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(2)>
		Public Overridable Property Product() As Product


		<Hidden>
		Public Property rowguid() As Guid

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"SpecialOfferProduct: {SpecialOfferID}-{ProductID}"
		End Function
	End Class
End Namespace