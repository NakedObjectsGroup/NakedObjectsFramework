Namespace AW.Types

	Partial Public Class ShoppingCartItem
		<Hidden>
		Public Property ShoppingCartItemID() As Integer

		<Hidden>
		Public Property ShoppingCartID() As String = ""

		<MemberOrder(20)>
		Public Property Quantity() As Integer

		<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		<Hidden>
		Public Property DateCreated() As DateTime

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"{Quantity}  x {Product}"
		End Function
	End Class
End Namespace