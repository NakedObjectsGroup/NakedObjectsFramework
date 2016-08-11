using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductProductPhotoMap : EntityTypeConfiguration<ProductProductPhoto>
    {
        public ProductProductPhotoMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.ProductPhotoID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductPhotoID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductProductPhoto", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            Property(t => t.Primary).HasColumnName("Primary");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product)
                .WithMany(t => t.ProductProductPhoto)
                .HasForeignKey(d => d.ProductID);
            HasRequired(t => t.ProductPhoto)
                .WithMany(t => t.ProductProductPhoto)
                .HasForeignKey(d => d.ProductPhotoID);

        }
    }
}
