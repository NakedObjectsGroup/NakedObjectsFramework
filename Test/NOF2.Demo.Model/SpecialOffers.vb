Namespace AW.Types

    Public Class SpecialOffers

        Public Shared Function ActionRandomSpecialOffer() As SpecialOffer
            Return GenericMenuFunctions.Random(Of SpecialOffer)()
        End Function

        Public Shared Function ActionAllSpecialOffers() As IQueryable(Of SpecialOffer)
            Return GenericMenuFunctions.ListAll(Of SpecialOffer)()
        End Function

        Public Shared Function ActionRecentlyUpdatedSpecialOffers() As IQueryable(Of SpecialOffer)
            Return From s In ActionAllSpecialOffers()
                   Order By s.mappedModifiedDate Descending
        End Function

        Public Shared Function ActionCreateNewSpecialOffer() As SpecialOffer
            Return CType(ThreadLocals.Container.CreateTransientInstance(GetType(SpecialOffer)), SpecialOffer)
        End Function

        Public Shared Function ActionAllSpecialOfferProducts() As IQueryable(Of SpecialOfferProduct)
            Return From sop In GenericMenuFunctions.ListAll(Of SpecialOfferProduct)()
                   Order By sop.mappedModifiedDate Descending
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Special Offers")
            main.AddAction(NameOf(ActionRandomSpecialOffer)) _
            .AddAction(NameOf(ActionAllSpecialOffers)) _
            .AddAction(NameOf(ActionRecentlyUpdatedSpecialOffers)) _
            .AddAction(NameOf(ActionCreateNewSpecialOffer)) _
            .AddAction(NameOf(ActionAllSpecialOfferProducts))
            Return main
        End Function

    End Class
End Namespace
