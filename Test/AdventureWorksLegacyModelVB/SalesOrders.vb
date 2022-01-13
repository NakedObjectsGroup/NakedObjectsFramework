Namespace AW.Types

    Public Class SalesOrders

        Public Shared Function ActionRandomSalesOrder() As SalesOrderHeader
            Return GenericMenuFunctions.Random(Of SalesOrderHeader)()
        End Function

        Public Shared Function ActionAllSalesOrders() As IQueryable(Of SalesOrderHeader)
            Return GenericMenuFunctions.ListAll(Of SalesOrderHeader)()
        End Function


    End Class
End Namespace
