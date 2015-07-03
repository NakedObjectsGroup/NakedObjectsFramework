namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.SalesTerritory")]
    public partial class SalesTerritory
    {
        public SalesTerritory()
        {
            StateProvinces = new HashSet<StateProvince>();
            Customers = new HashSet<Customer>();
            SalesOrderHeaders = new HashSet<SalesOrderHeader>();
            SalesPersons = new HashSet<SalesPerson>();
            SalesTerritoryHistories = new HashSet<SalesTerritoryHistory>();
        }

        [Key]
        public int TerritoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(3)]
        public string CountryRegionCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Group { get; set; }

        [Column(TypeName = "money")]
        public decimal SalesYTD { get; set; }

        [Column(TypeName = "money")]
        public decimal SalesLastYear { get; set; }

        [Column(TypeName = "money")]
        public decimal CostYTD { get; set; }

        [Column(TypeName = "money")]
        public decimal CostLastYear { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<StateProvince> StateProvinces { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }

        public virtual ICollection<SalesPerson> SalesPersons { get; set; }

        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories { get; set; }
    }
}
