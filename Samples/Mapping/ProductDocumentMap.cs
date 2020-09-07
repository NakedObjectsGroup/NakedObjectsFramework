using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductDocumentMap : EntityTypeConfiguration<ProductDocument>
    {
        public ProductDocumentMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.DocumentID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.DocumentID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductDocument", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.DocumentID).HasColumnName("DocumentID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Document).WithMany().HasForeignKey(t => t.DocumentID);
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }
}
