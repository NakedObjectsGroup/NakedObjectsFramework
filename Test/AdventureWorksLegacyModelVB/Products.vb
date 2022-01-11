Namespace AW.Types
    'Test retrieval of all types relating to Product
    Public Class Products

        Public Shared Function ActionRandomProduct() As Product
            Return SimpleRepository.Random(Of Product)()
        End Function

        Public Shared Function ActionRandomBillOfMaterial() As BillOfMaterial
            Return SimpleRepository.Random(Of BillOfMaterial)()
        End Function

        Public Shared Function ActionRandomBusinessEntityAddress() As BusinessEntityAddress
            Return SimpleRepository.Random(Of BusinessEntityAddress)()
        End Function

        Public Shared Function ActionRandomBusinessEntityContact() As BusinessEntityContact
            Return SimpleRepository.Random(Of BusinessEntityContact)()
        End Function


    End Class
End Namespace
