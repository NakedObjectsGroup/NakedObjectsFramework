using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using static NakedFunctions.Helpers;


using System.Linq.Expressions;

namespace AdventureWorksModel.Sales
{
    public static class SpecialOfferContributedActions
    {
        private static (IList<SpecialOffer>, IList<SpecialOffer>) Change(this IQueryable<SpecialOffer> offers, Func<SpecialOffer,SpecialOffer> change)
=> DisplayAndPersist(offers.ToList().Select(change).ToList());

        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static (IList<SpecialOffer>, IList<SpecialOffer>) ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate)
        => Change(offers, x => x with { EndDate = toDate });


        public static (IList<SpecialOffer>, IList<SpecialOffer>) TerminateActiveOffers(
            this IQueryable<SpecialOffer> offers,
            [Injected] DateTime now)
        {
            var yesterday = now.Date.AddDays(-1);
            var list = offers.Where(x => x.EndDate > yesterday).ToList().Select(x => x with { EndDate = yesterday }).ToList();
            return DisplayAndPersist(list);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeType(
            this IQueryable<SpecialOffer> offers,
            string newType)
=> Change(offers, x => x with { Type = newType });


        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeMaxQuantity(
            this IQueryable<SpecialOffer> offers,
            int newMax)
        => Change(offers, x => x with { MaxQty = newMax });


        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeDiscount(this IQueryable<SpecialOffer> offers, decimal newDiscount)
        => Change(offers, x => x with { DiscountPct = newDiscount });
    }
}
