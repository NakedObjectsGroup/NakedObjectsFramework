Namespace AW.Types
    'Test retrieval of all types relating to Product
    Public Class Products

        Public Shared Function ActionRandomProduct() As Product
            Return GenericMenuFunctions.Random(Of Product)()
        End Function

        Public Shared Function ActionAllProducts() As IQueryable(Of Product)
            Return GenericMenuFunctions.ListAll(Of Product)()
        End Function

    End Class
End Namespace
