Namespace AW.Types

    Public Class SpecialOffers

        Public Shared Function ActionRandomSpecialOffer() As SpecialOffer
            Return GenericMenuFunctions.Random(Of SpecialOffer)()
        End Function

        Public Shared Function ActionAllSpecialOffers() As IQueryable(Of SpecialOffer)
            Return GenericMenuFunctions.ListAll(Of SpecialOffer)()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Special Offers")
            main.AddAction(NameOf(ActionRandomSpecialOffer)) _
            .AddAction(NameOf(ActionAllSpecialOffers))
            Return main
        End Function

    End Class
End Namespace
