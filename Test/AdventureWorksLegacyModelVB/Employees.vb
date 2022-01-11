Namespace AW.Types

    Public Class Employees

        Public Shared Function ActionListAllDepartments() As ArrayList
            Return SimpleRepository.ListAll(Of Department).ToArrayList()
        End Function

        Public Shared Function ActionRandomEmployee() As Employee
            Return SimpleRepository.Random(Of Employee)()
        End Function

        Public Shared Function ActionFindEmployeeByName() As ArrayList
            Throw New NotImplementedException()
        End Function

        Public Shared Function ActionFindEmployeeByNationalIDNumber() As ArrayList
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace
