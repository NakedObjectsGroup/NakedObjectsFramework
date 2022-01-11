Namespace AW.Types

    Public Class Vendors

        'Public Shared Function ActionOrder() As String
        '    Return "FindVendorByName,FindVendorByAccountNumber,  Randomvendor"
        'End Function

        Public Shared Function ActionRandomVendor() As Vendor
            Return SimpleRepository.Random(Of Vendor)()
        End Function

        Public Shared Function ActionFindVendorByName() As ArrayList
            Throw New NotImplementedException()
        End Function

        Public Shared Function ActionFindVendorByAccountNumber() As ArrayList
            Throw New NotImplementedException()
        End Function



    End Class
End Namespace
