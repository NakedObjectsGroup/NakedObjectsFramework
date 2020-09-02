using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;
using NakedFunctions;
using static NakedFunctions.Result;
using System.Linq.Expressions;

namespace AdventureWorksModel.Sales {
    public static class SpecialOfferContributedActions
    {
        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static (IList<SpecialOffer> , IList<SpecialOffer>) ExtendOffers([ContributedAction] IQueryable<SpecialOffer> offers, DateTime toDate)
        {
            return Change(offers, y => y.EndDate, toDate);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) TerminateActiveOffers(
            [ContributedAction] IQueryable<SpecialOffer> offers,
            [Injected] DateTime now)
        {
            var yesterday = now.Date.AddDays(-1);
            return Change(offers.Where(x => x.EndDate > yesterday),y => y.EndDate, yesterday);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeType(
            [ContributedAction] IQueryable<SpecialOffer> offers, 
            string newType)
        {
            return Change(offers, y => y.Type, newType);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeMaxQuantity(
            [ContributedAction] IQueryable<SpecialOffer> offers, 
            int newMax)
        {
            return Change(offers, y => y.MaxQty, newMax);
        }

        public static (IList<SpecialOffer>, IList<SpecialOffer>) ChangeDiscount([ContributedAction] IQueryable<SpecialOffer> offers, decimal newDiscount)
        {
            return Change(offers, y => y.DiscountPct, newDiscount);
        }

        private static (IList<SpecialOffer>, IList<SpecialOffer>) Change<T>(IQueryable<SpecialOffer> offers, Expression<Func<SpecialOffer, T>> property, T value)
        {
            return DisplayAndPersist(offers.ToList().Select(x => x.With(property, value)).ToList());
        }
    }
}
