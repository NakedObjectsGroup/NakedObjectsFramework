Namespace AW.Types

    Public Class Cart

        Public Shared Function ActionRandomShoppingCartItem() As ShoppingCartItem
            Return GenericMenuFunctions.Random(Of ShoppingCartItem)()
        End Function

    End Class
End Namespace
