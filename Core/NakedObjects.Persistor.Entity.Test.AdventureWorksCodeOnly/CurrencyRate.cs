namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.CurrencyRate")]
    public partial class CurrencyRate
    {
        public CurrencyRate()
        {
            SalesOrderHeaders = new HashSet<SalesOrderHeader>();
        }

        public int CurrencyRateID { get; set; }

        public DateTime CurrencyRateDate { get; set; }

        [Required]
        [StringLength(3)]
        public string FromCurrencyCode { get; set; }

        [Required]
        [StringLength(3)]
        public string ToCurrencyCode { get; set; }

        [Column(TypeName = "money")]
        public decimal AverageRate { get; set; }

        [Column(TypeName = "money")]
        public decimal EndOfDayRate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual Currency Currency1 { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}
