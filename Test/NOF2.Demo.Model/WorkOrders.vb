Namespace AW.Types

    Public Class WorkOrders

        Public Shared Function ActionRandomWorkOrder() As WorkOrder
            Return GenericMenuFunctions.Random(Of WorkOrder)()
        End Function

        Public Shared Function ActionAllWorkOrders() As IQueryable(Of WorkOrder)
            Return GenericMenuFunctions.ListAll(Of WorkOrder)()
        End Function

        Public Shared Function ActionCurrentWorkOrders() As IQueryable(Of WorkOrder)
            Return From w In ThreadLocals.Container.AllInstances(Of WorkOrder)()
                   Where w.mappedEndDate Is Nothing
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Work Orders")
            main.AddAction(NameOf(ActionRandomWorkOrder)) _
            .AddAction("ActionCurrentWorkOrders") _ 'To test that Action prefix is not added if already there
            .AddAction("AllWorkOrders") 'To test that Action prefix is added & name is case insensitive
            Return main
        End Function

    End Class
End Namespace
