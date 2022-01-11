Namespace AW.Types

    Public Class Employees

        <MemberOrder(4)>
        Public Shared Function ActionListAllDepartments() As ArrayList
            Return SimpleRepository.ListAll(Of Department).ToArrayList()
        End Function

        <MemberOrder(3)>
        Public Shared Function ActionRandomEmployee() As Employee
            Return SimpleRepository.Random(Of Employee)()
        End Function

        <MemberOrder(1)>
        Public Shared Function ActionFindEmployeeByName() As ArrayList
            Throw New NotImplementedException()
        End Function

        <MemberOrder(2)>
        Public Shared Function ActionFindEmployeeByNationalIDNumber() As ArrayList
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace
