Namespace AW.Types

    Public Class Employee_Menu

        Public Shared Function ActionListAllDepartments() As ArrayList
            Return SimpleRepository.ListAll(Of Department).ToArrayList()
        End Function

        Public Shared Function ActionRandomEmployee() As Employee
            Return SimpleRepository.Random(Of Employee)()
        End Function

    End Class
End Namespace
