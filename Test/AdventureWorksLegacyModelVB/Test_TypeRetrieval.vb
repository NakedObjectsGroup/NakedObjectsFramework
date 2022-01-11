Namespace AW.Types
    'Test retrieval of all types
    Public Class Test_TypeRetrieval

        Public Shared Function ActionRandomEmployeeDepartmentHistory() As EmployeeDepartmentHistory
            Return SimpleRepository.Random(Of EmployeeDepartmentHistory)()
        End Function

        Public Shared Function ActionRandomEmployeePayHistory() As EmployeePayHistory
            Return SimpleRepository.Random(Of EmployeePayHistory)()
        End Function

        Public Shared Function ActionRandomBusinessEntityAddress() As BusinessEntityAddress
            Return SimpleRepository.Random(Of BusinessEntityAddress)()
        End Function

        Public Shared Function ActionRandomBusinessEntityContact() As BusinessEntityContact
            Return SimpleRepository.Random(Of BusinessEntityContact)()
        End Function

        Public Shared Function ActionRandomTransactionHistory() As TransactionHistory
            Return SimpleRepository.Random(Of TransactionHistory)()
        End Function

        Public Shared Function ActionRandomPassword() As Password
            Return SimpleRepository.Random(Of Password)()
        End Function

        Public Shared Function ActionRandomCurrency() As Currency
            Return SimpleRepository.Random(Of Currency)()
        End Function

        Public Shared Function ActionRandomBillOfMaterial() As BillOfMaterial
            Return SimpleRepository.Random(Of BillOfMaterial)()
        End Function

        Public Shared Function ActionRandomSalesTerritoryHistory() As SalesTerritoryHistory
            Return SimpleRepository.Random(Of SalesTerritoryHistory)()
        End Function


    End Class
End Namespace
