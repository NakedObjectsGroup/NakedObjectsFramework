using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class Document
    {
        public Document()
        {
            this.ProductDocuments = new List<ProductDocument>();
        }

        public int DocumentID { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string Revision { get; set; }
        public int ChangeNumber { get; set; }
        public byte Status { get; set; }
        public string DocumentSummary { get; set; }
        public byte[] Document1 { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<ProductDocument> ProductDocuments { get; set; }
    }
}
