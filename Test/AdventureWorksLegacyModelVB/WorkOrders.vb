Namespace AW.Types

    Public Class WorkOrders

        Public Shared Function ActionRandomWorkOrder() As WorkOrder
            Return SimpleRepository.Random(Of WorkOrder)()
        End Function

        Public Shared Function ActionCurrentWorkOrders() As IQueryable(Of WorkOrder)
            Return From w In ThreadLocals.Container.Instances(Of WorkOrder)()
                   Where w.mappedEndDate Is Nothing
        End Function

    End Class
End Namespace
