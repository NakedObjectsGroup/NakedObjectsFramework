Namespace AW.Types

    Public Class Vendors


        Public Shared Function ActionRandomVendor() As Vendor
            Return SimpleRepository.Random(Of Vendor)()
        End Function

        Public Shared Function ActionRandomProductVendor() As ProductVendor
            Return SimpleRepository.Random(Of ProductVendor)()
        End Function

    End Class
End Namespace
