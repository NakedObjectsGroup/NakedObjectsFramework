Namespace AW.Types

    Public Class Employees
        Private Shared Function Employees() As IQueryable(Of Employee)
            Return ThreadLocals.Container.Instances(Of Employee)
        End Function
        Public Shared Function ActionListAllDepartments() As ArrayList
            Return GenericMenuFunctions.ListAll(Of Department).ToArrayList()
        End Function

        Public Shared Function ActionRandomEmployee() As Employee
            Return GenericMenuFunctions.Random(Of Employee)()
        End Function

        Public Shared Function ActionAllEmployees() As IQueryable(Of Employee)
            Return GenericMenuFunctions.ListAll(Of Employee)()
        End Function

        Public Shared Function ActionFindEmployeeByName(firstName As TextString, lastName As TextString) As IQueryable(Of Employee)
            Return From e In Employees()
                   Where e.PersonDetails.mappedLastName.ToUpper().StartsWith(lastName.Value) AndAlso
                       (firstName.Value Is "" OrElse e.PersonDetails.mappedFirstName.ToUpper().StartsWith(firstName.Value))
        End Function

        Public Shared Function ActionFindEmployeeByNationalIDNumber(nationalIDNumber As TextString) As Employee
            Return (From e In Employees()
                    Where e.mappedNationalIDNumber = nationalIDNumber.Value).FirstOrDefault()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Employees")
            main.AddAction(NameOf(ActionRandomEmployee)) _
            .AddAction(NameOf(ActionAllEmployees)) _
            .AddAction(NameOf(ActionFindEmployeeByName)) _
            .AddAction(NameOf(ActionFindEmployeeByNationalIDNumber))

            main.AddSubMenu("Organisation").AddAction(NameOf(ActionListAllDepartments))
            Return main
        End Function

    End Class
End Namespace
