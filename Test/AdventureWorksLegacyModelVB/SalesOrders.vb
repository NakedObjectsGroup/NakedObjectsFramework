Namespace AW.Types

    Public Class SalesOrders

        Public Shared Function ActionRandomSalesOrderHeader() As SalesOrderHeader
            Return SimpleRepository.Random(Of SalesOrderHeader)()
        End Function

        Public Shared Function ActionRandomSalesOrderDetail() As SalesOrderDetail
            Return SimpleRepository.Random(Of SalesOrderDetail)()
        End Function

    End Class
End Namespace
