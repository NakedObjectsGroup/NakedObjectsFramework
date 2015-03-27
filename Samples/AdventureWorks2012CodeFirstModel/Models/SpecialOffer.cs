using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class SpecialOffer
    {
        public SpecialOffer()
        {
            this.SpecialOfferProducts = new List<SpecialOfferProduct>();
        }

        public int SpecialOfferID { get; set; }
        public string Description { get; set; }
        public decimal DiscountPct { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int MinQty { get; set; }
        public Nullable<int> MaxQty { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts { get; set; }
    }
}
