using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel.Sales
{
    public class SpecialOfferContributedActions
    {

        public void ExtendOffers([ContributedAction] IQueryable<SpecialOffer> offers, DateTime toDate)
        {
            foreach (SpecialOffer offer in offers)
            {
                if (offer.EndDate < toDate)
                {
                    offer.EndDate = toDate;
                }
            }
        }

        public void TerminateActiveOffers([ContributedAction] IQueryable<SpecialOffer> offers)
        {
            foreach (SpecialOffer offer in offers)
            {
                var yesterday = DateTime.Today.AddDays(-1);
                if (offer.EndDate > yesterday) //i.e. only terminate offers not already completed
                {
                    offer.EndDate = yesterday;
                }
            }
        }

        public void ChangeType(
            [ContributedAction] IQueryable<SpecialOffer> offers, 
            string newType)
        {
            foreach (SpecialOffer offer in offers)
            {
                offer.Type = newType;
            }
        }


        public void ChangeMaxQuantity([ContributedAction] IQueryable<SpecialOffer> offers, int newMax)
        {
            foreach (SpecialOffer offer in offers)
            {
                offer.MaxQty = newMax;
            }
        }

    }
}
