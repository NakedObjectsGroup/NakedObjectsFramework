using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class ProductDocumentMap : EntityTypeConfiguration<ProductDocument>
    {
        public ProductDocumentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.DocumentID });

            // Properties
            this.Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DocumentID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ProductDocument", "Production");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.DocumentID).HasColumnName("DocumentID");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Document)
                .WithMany(t => t.ProductDocuments)
                .HasForeignKey(d => d.DocumentID);
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProductDocuments)
                .HasForeignKey(d => d.ProductID);

        }
    }
}
