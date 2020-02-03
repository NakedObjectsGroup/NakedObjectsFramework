namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.SpecialOffer")]
    public partial class SpecialOffer
    {
        private ICollection<SpecialOfferProduct> specialOfferProducts = new List<SpecialOfferProduct>();

        public int SpecialOfferID { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal DiscountPct { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MinQty { get; set; }

        public int? MaxQty { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts {
            get { return specialOfferProducts; }
            set { specialOfferProducts = value; }
        }
    }
}
