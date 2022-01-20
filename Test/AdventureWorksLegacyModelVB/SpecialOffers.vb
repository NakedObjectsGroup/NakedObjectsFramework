Namespace AW.Types

    Public Class SpecialOffers

        Public Shared Function ActionRandomSpecialOffer() As SpecialOffer
            Return GenericMenuFunctions.Random(Of SpecialOffer)()
        End Function

        Public Shared Function ActionAllSpecialOffers() As IQueryable(Of SpecialOffer)
            Return GenericMenuFunctions.ListAll(Of SpecialOffer)()
        End Function

        Public Shared Function ActionCreateNewSpecialOffer() As SpecialOffer
            Return CType(ThreadLocals.Container.CreateTransientInstance(GetType(SpecialOffer)), SpecialOffer)
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Special Offers")
            main.AddAction(NameOf(ActionRandomSpecialOffer)) _
            .AddAction(NameOf(ActionAllSpecialOffers)) _
            .AddAction(NameOf(ActionCreateNewSpecialOffer))
            Return main
        End Function

    End Class
End Namespace
