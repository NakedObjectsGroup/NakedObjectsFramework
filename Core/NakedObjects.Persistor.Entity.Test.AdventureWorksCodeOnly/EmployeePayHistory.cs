namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HumanResources.EmployeePayHistory")]
    public partial class EmployeePayHistory
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeID { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime RateChangeDate { get; set; }

        [Column(TypeName = "money")]
        public decimal Rate { get; set; }

        public byte PayFrequency { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
