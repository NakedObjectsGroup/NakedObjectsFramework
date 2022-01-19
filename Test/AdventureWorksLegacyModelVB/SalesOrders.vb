Namespace AW.Types

    Public Class SalesOrders

        Public Shared Function ActionRandomSalesOrder() As SalesOrderHeader
            Return GenericMenuFunctions.Random(Of SalesOrderHeader)()
        End Function

        Public Shared Function ActionAllSalesOrders() As IQueryable(Of SalesOrderHeader)
            Return GenericMenuFunctions.ListAll(Of SalesOrderHeader)()
        End Function

        Public Shared Function ActionSalesOrdersByStatus(status As WholeNumber) As IQueryable(Of SalesOrderHeader)
            Dim b As Byte = CType(status.Value, Byte)
            Return From s In ActionAllSalesOrders()
                   Where s.Status = b
        End Function


        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Sales Orders")
            main.AddAction(NameOf(ActionRandomSalesOrder)) _
            .AddAction(NameOf(ActionAllSalesOrders)) _
            .AddAction(NameOf(ActionSalesOrdersByStatus))
            Return main
        End Function
    End Class
End Namespace
