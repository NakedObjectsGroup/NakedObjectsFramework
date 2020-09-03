using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;


using System.Linq.Expressions;

namespace AdventureWorksModel.Sales
{
    public static class SpecialOfferContributedActions
    {
        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static (IList<SpecialOffer>, IList<SpecialOffer>) ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate)
        {
            var list = offers.ToList().Select(x => x with { EndDate = toDate }).ToList();
            return (list, list);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) TerminateActiveOffers(
            this IQueryable<SpecialOffer> offers,
            [Injected] DateTime now)
        {
            var yesterday = now.Date.AddDays(-1);
            var list = offers.Where(x => x.EndDate > yesterday).ToList().Select(x => x with { EndDate = yesterday }).ToList();
            return (list, list);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeType(
            this IQueryable<SpecialOffer> offers,
            string newType)
        {
            throw new NotImplementedException(); //see examples above
                                                 // return Change(offers, y => y.Type, newType);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeMaxQuantity(
            this IQueryable<SpecialOffer> offers,
            int newMax)
        {
            throw new NotImplementedException(); //see examples above
            // returnreturn Change(offers, y => y.MaxQty, newMax);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeDiscount(this IQueryable<SpecialOffer> offers, decimal newDiscount)
        {
            throw new NotImplementedException(); //see examples above
            //return Change(offers, y => y.DiscountPct, newDiscount);
        }

    }
}
