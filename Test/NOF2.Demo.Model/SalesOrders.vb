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

        Public Shared Function ActionFindSalesOrder(number As TextString) As SalesOrderHeader
            Dim num = number.Value
            Return (From o In ActionAllSalesOrders()
                    Where o.mappedAccountNumber = num).FirstOrDefault()
        End Function

        Public Shared Sub AboutActionFindSalesOrder(a As ActionAbout, number As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Parameters
                    'a.ParamDefaultValues(0) = "SO"
                    'a.ParamLabels(0) = "Order Number"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                    If number.Value.Length < 3 OrElse
                            Not number.Value.Substring(0, 2) = "SO" Then
                        a.Usable = False
                        a.UnusableReason = "Number must begin with SO"
                    End If
                Case Else
            End Select
        End Sub

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Sales Orders")
            main.AddAction(NameOf(ActionRandomSalesOrder)) _
            .AddAction(NameOf(ActionAllSalesOrders)) _
            .AddAction(NameOf(ActionSalesOrdersByStatus)) _
            .AddAction(NameOf(ActionFindSalesOrder))
            Return main
        End Function
    End Class
End Namespace
