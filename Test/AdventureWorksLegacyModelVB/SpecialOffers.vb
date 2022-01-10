Namespace AW.Types

    Public Class SpecialOffers

        Public Shared Function ActionRandomSpecialOffer() As SpecialOffer
            Return SimpleRepository.Random(Of SpecialOffer)()
        End Function

    End Class
End Namespace
