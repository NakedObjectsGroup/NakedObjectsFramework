Namespace AW.Types

    Public Class WorkOrders

        Public Shared Function ActionRandomWorkOrder() As WorkOrder
            Return SimpleRepository.Random(Of WorkOrder)()
        End Function

        Public Shared Function ActionRandomWorkOrderRouting() As WorkOrderRouting
            Return SimpleRepository.Random(Of WorkOrderRouting)()
        End Function

    End Class
End Namespace
