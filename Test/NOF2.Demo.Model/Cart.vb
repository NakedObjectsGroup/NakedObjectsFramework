Namespace AW.Types

    Public Class Cart

        Public Shared Function ActionRandomShoppingCartItem() As ShoppingCartItem
            Return GenericMenuFunctions.Random(Of ShoppingCartItem)()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Foo")
            'main.AddAction(NameOf(ActionRandomShoppingCartItem))
            Return main
        End Function
    End Class
End Namespace
