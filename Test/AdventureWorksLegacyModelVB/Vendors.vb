Namespace AW.Types

    Public Class Vendors


        Public Shared Function ActionRandomVendor() As Vendor
            Return GenericMenuFunctions.Random(Of Vendor)()
        End Function

        Public Shared Function ActionAllVendors() As IQueryable(Of Vendor)
            Return GenericMenuFunctions.ListAll(Of Vendor)()
        End Function

        Public Shared Function ActionFindVendorByName() As ArrayList
            Throw New NotImplementedException()
        End Function

        Public Shared Function ActionFindVendorByAccountNumber() As ArrayList
            Throw New NotImplementedException()
        End Function



    End Class
End Namespace
