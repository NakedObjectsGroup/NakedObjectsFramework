namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.Currency")]
    public partial class Currency
    {
        public Currency()
        {
            CountryRegionCurrencies = new HashSet<CountryRegionCurrency>();
            CurrencyRates = new HashSet<CurrencyRate>();
            CurrencyRates1 = new HashSet<CurrencyRate>();
        }

        [Key]
        [StringLength(3)]
        public string CurrencyCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<CountryRegionCurrency> CountryRegionCurrencies { get; set; }

        public virtual ICollection<CurrencyRate> CurrencyRates { get; set; }

        public virtual ICollection<CurrencyRate> CurrencyRates1 { get; set; }
    }
}
