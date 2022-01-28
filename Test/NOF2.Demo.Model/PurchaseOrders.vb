Namespace AW.Types

    Public Class PurchaseOrders

        Public Shared Function ActionRandomPurchaseOrderHeader() As PurchaseOrderHeader
            Return GenericMenuFunctions.Random(Of PurchaseOrderHeader)()
        End Function

        Public Shared Function ActionAllPurchaseOrders() As IQueryable(Of PurchaseOrderHeader)
            Return GenericMenuFunctions.ListAll(Of PurchaseOrderHeader)()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Purchase Orders")
            main.AddAction(NameOf(ActionRandomPurchaseOrderHeader)) _
            .AddAction(NameOf(ActionAllPurchaseOrders))
            Return main
        End Function

    End Class
End Namespace
