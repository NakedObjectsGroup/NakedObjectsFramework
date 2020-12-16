using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductModelIllustrationMap : EntityTypeConfiguration<ProductModelIllustration>
    {
        public ProductModelIllustrationMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductModelID, t.IllustrationID });

            // Properties
            Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.IllustrationID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductModelIllustration", "Production");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Illustration)
                .WithMany(t => t.ProductModelIllustration)
                .HasForeignKey(d => d.IllustrationID);
            HasRequired(t => t.ProductModel)
                .WithMany(t => t.ProductModelIllustration)
                .HasForeignKey(d => d.ProductModelID);

        }
    }
}
