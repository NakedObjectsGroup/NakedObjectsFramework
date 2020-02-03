namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.Document")]
    public partial class Document
    {
        public Document()
        {
            ProductDocuments = new HashSet<ProductDocument>();
        }

        public int DocumentID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(400)]
        public string FileName { get; set; }

        [Required]
        [StringLength(8)]
        public string FileExtension { get; set; }

        [Required]
        [StringLength(5)]
        public string Revision { get; set; }

        public int ChangeNumber { get; set; }

        public byte Status { get; set; }

        public string DocumentSummary { get; set; }

        [Column("Document")]
        public byte[] Document1 { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductDocument> ProductDocuments { get; set; }
    }
}
