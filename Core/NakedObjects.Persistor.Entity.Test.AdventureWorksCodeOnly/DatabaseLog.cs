namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatabaseLog")]
    public partial class DatabaseLog
    {
        public int DatabaseLogID { get; set; }

        public DateTime PostTime { get; set; }

        [Required]
        [StringLength(128)]
        public string DatabaseUser { get; set; }

        [Required]
        [StringLength(128)]
        public string Event { get; set; }

        [StringLength(128)]
        public string Schema { get; set; }

        [StringLength(128)]
        public string Object { get; set; }

        [Required]
        public string TSQL { get; set; }

        [Column(TypeName = "xml")]
        [Required]
        public string XmlEvent { get; set; }
    }
}
