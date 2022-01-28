Namespace AW.Types
    'Test retrieval of all types relating to Product
    Public Class Products

        Public Shared Function ActionRandomProduct() As Product
            Return GenericMenuFunctions.Random(Of Product)()
        End Function

        Public Shared Function ActionAllProducts() As IQueryable(Of Product)
            Return GenericMenuFunctions.ListAll(Of Product)()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Products")
            main.AddAction(NameOf(ActionRandomProduct)) _
            .AddAction(NameOf(ActionAllProducts))
            Return main
        End Function

    End Class
End Namespace
