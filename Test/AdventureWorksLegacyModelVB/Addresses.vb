Namespace AW.Types

    Public Class Addresses

        Public Shared Function ActionRandomAddress() As Address
            Return GenericMenuFunctions.Random(Of Address)()
        End Function

        Public Shared Function ActionAllAddresses() As IQueryable(Of Address)
            Return GenericMenuFunctions.ListAll(Of Address)()
        End Function

    End Class
End Namespace
