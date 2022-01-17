Namespace AW.Types

    Public Class WorkOrders

        Public Shared Function ActionRandomWorkOrder() As WorkOrder
            Return GenericMenuFunctions.Random(Of WorkOrder)()
        End Function

        Public Shared Function ActionAllWorkOrders() As IQueryable(Of WorkOrder)
            Return GenericMenuFunctions.ListAll(Of WorkOrder)()
        End Function

        Public Shared Function ActionCurrentWorkOrders() As IQueryable(Of WorkOrder)
            Return From w In ThreadLocals.Container.Instances(Of WorkOrder)()
                   Where w.mappedEndDate Is Nothing
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Work Orders")
            main.AddAction(NameOf(ActionRandomWorkOrder)) _
            .AddAction("AllWorkOrders") _ 'To test that Action prefix is added
            .AddAction("actionAllWorkOrders") 'To test that Action prefix is not added if already there
            Return main
        End Function

    End Class
End Namespace
