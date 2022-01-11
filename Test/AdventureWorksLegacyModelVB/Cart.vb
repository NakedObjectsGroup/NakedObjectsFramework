Namespace AW.Types

    Public Class Cart

        Public Shared Function ActionRandomShoppingCartItem() As ShoppingCartItem
            Return SimpleRepository.Random(Of ShoppingCartItem)()
        End Function

    End Class
End Namespace
