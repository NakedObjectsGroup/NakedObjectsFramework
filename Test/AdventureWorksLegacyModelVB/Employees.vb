Namespace AW.Types

    Public Class Employees

        Public Shared Function ActionListAllDepartments() As ArrayList
            Return GenericMenuFunctions.ListAll(Of Department).ToArrayList()
        End Function

        Public Shared Function ActionRandomEmployee() As Employee
            Return GenericMenuFunctions.Random(Of Employee)()
        End Function

        Public Shared Function ActionAllEmployees() As IQueryable(Of Employee)
            Return GenericMenuFunctions.ListAll(Of Employee)()
        End Function

        Public Shared Function ActionFindEmployeeByName(firstName As TextString, lastName As TextString) As ArrayList
            Throw New NotImplementedException()
        End Function

        Public Shared Function ActionFindEmployeeByNationalIDNumber() As ArrayList
            Throw New NotImplementedException()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Employees")
            main.AddAction(NameOf(ActionListAllDepartments)) _
            .AddAction(NameOf(ActionRandomEmployee)) _
            .AddAction(NameOf(ActionAllEmployees)) _
            .AddAction(NameOf(ActionFindEmployeeByName)) _
            .AddAction(NameOf(ActionFindEmployeeByNationalIDNumber))
            Return main
        End Function

    End Class
End Namespace
