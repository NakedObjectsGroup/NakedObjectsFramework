Namespace AW.Types

    Public Class Employees

        Public Shared Function ActionListAllDepartments() As ArrayList
            Return GenericMenuFunctions.ListAll(Of Department).ToArrayList()
        End Function

        Public Shared Function ActionRandomEmployee() As Employee
            Return GenericMenuFunctions.Random(Of Employee)()
        End Function

        Public Shared Function ActionFindEmployeeByName() As ArrayList
            Throw New NotImplementedException()
        End Function

        Public Shared Function ActionFindEmployeeByNationalIDNumber() As ArrayList
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace
