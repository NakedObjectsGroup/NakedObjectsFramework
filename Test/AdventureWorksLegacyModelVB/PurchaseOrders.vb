Namespace AW.Types

    Public Class PurchaseOrders

        Public Shared Function ActionRandomPurchaseOrderHeader() As PurchaseOrderHeader
            Return GenericMenuFunctions.Random(Of PurchaseOrderHeader)()
        End Function

        Public Shared Function ActionAllPurchaseOrders() As IQueryable(Of PurchaseOrderHeader)
            Return GenericMenuFunctions.ListAll(Of PurchaseOrderHeader)()
        End Function

    End Class
End Namespace
