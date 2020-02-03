namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HumanResources.JobCandidate")]
    public partial class JobCandidate
    {
        public int JobCandidateID { get; set; }

        public int? EmployeeID { get; set; }

        [Column(TypeName = "xml")]
        public string Resume { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
