Namespace AW.Functions

	<Named("Cart")>
	Public Class ShoppingCart_Functions
		Public Function RemoveItems(ByVal items As IQueryable(Of ShoppingCartItem), ByVal context As IContext) As IQueryable(Of ShoppingCartItem)
			ShoppingCart_MenuFunctions.RemoveItems(items, context)
			Return ShoppingCart_MenuFunctions.Cart(context)
		End Function

		Public Function AddToCart(ByVal product As Product, ByVal context As IContext) As IContext
			Return ShoppingCart_MenuFunctions.AddToShoppingCart(product, context)
		End Function

'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
'ORIGINAL LINE: public string? DisableAddToCart(IContext context)
		Public Function DisableAddToCart(ByVal context As IContext) As String
			Return ShoppingCart_MenuFunctions.DisableIfNoCustomerForUser(context)
		End Function
	End Class
End Namespace