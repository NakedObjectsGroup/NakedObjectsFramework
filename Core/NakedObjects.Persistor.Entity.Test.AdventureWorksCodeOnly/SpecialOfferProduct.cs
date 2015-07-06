namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.SpecialOfferProduct")]
    public partial class SpecialOfferProduct
    {
        private ICollection<SalesOrderDetail> salesOrderDetails = new List<SalesOrderDetail>();     

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SpecialOfferID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductID { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails {
            get { return salesOrderDetails; }
            set { salesOrderDetails = value; }
        }

        public virtual SpecialOffer SpecialOffer { get; set; }
    }
}
