Namespace AW.Types

    Public Class SalesOrders

        Public Shared Function ActionRandomSalesOrderHeader() As SalesOrderHeader
            Return GenericMenuFunctions.Random(Of SalesOrderHeader)()
        End Function

        Public Shared Function ActionRandomSalesOrderDetail() As SalesOrderDetail
            Return GenericMenuFunctions.Random(Of SalesOrderDetail)()
        End Function

    End Class
End Namespace
