namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.SalesTaxRate")]
    public partial class SalesTaxRate
    {
        public int SalesTaxRateID { get; set; }

        public int StateProvinceID { get; set; }

        public byte TaxType { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal TaxRate { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual StateProvince StateProvince { get; set; }
    }
}
