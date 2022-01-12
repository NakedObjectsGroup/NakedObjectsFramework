Namespace AW.Types

    Public Class PurchaseOrders

        Public Shared Function ActionRandomPurchaseOrderHeader() As PurchaseOrderHeader
            Return GenericMenuFunctions.Random(Of PurchaseOrderHeader)()
        End Function

        Public Shared Function ActionRandomPurchaseOrderDetail() As PurchaseOrderDetail
            Return GenericMenuFunctions.Random(Of PurchaseOrderDetail)()
        End Function

    End Class
End Namespace
