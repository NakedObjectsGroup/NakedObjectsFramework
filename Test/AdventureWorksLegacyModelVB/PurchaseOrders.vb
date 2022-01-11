Namespace AW.Types

    Public Class PurchaseOrders

        Public Shared Function ActionRandomPurchaseOrderHeader() As PurchaseOrderHeader
            Return SimpleRepository.Random(Of PurchaseOrderHeader)()
        End Function

        Public Shared Function ActionRandomPurchaseOrderDetail() As PurchaseOrderDetail
            Return SimpleRepository.Random(Of PurchaseOrderDetail)()
        End Function

    End Class
End Namespace
