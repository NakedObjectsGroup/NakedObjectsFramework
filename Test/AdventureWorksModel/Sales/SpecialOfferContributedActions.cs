using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel.Sales {
    public class SpecialOfferContributedActions
    {

        public IDomainObjectContainer Container { set; protected get; }


        public void ExtendOffers([ContributedAction] IQueryable<SpecialOffer> offers, DateTime toDate)
        {
            foreach (SpecialOffer offer in offers)
            {
                    offer.EndDate = toDate;
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

        public void ChangeDiscount([ContributedAction] IQueryable<SpecialOffer> offers, decimal newDiscount)
        {
            foreach (SpecialOffer offer in offers)
            {
                offer.DiscountPct = newDiscount;
            }
        }

        //To test an empty param
        public void AppendToDescription([ContributedAction] IQueryable<SpecialOffer> offers, [Optionally] string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            foreach (SpecialOffer offer in offers)
            {
                offer.Description += text;
            }
        }

        [DisplayAsProperty]
        public ICollection<Product> Products([ContributedAction] SpecialOffer so)
        {
            int soid = so.SpecialOfferID;
            var prods = Container.Instances<Product>();
            var sops = Container.Instances<SpecialOfferProduct>();
            return (from p in prods
                    from sop in sops
                    where sop.SpecialOfferID == soid && p.ProductID == sop.ProductID
                    select p).ToList();
        }
    }
}
