Namespace AW.Types

    Public Class Addresses

        Public Shared Function ActionRandomAddress() As Address
            Return GenericMenuFunctions.Random(Of Address)()
        End Function

    End Class
End Namespace
