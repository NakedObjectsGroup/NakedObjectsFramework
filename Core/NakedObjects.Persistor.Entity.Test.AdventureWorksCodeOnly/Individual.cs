namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.Individual")]
    public partial class Individual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        public int ContactID { get; set; }

        [Column(TypeName = "xml")]
        public string Demographics { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
