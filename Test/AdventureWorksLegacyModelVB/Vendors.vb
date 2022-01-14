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

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Vendors")
            main.AddAction(NameOf(ActionRandomVendor)) _
            .AddAction(NameOf(ActionAllVendors)) _
            .AddAction(NameOf(ActionFindVendorByAccountNumber)) _
            .AddAction(NameOf(ActionFindVendorByName))
            Return main
        End Function

    End Class
End Namespace
