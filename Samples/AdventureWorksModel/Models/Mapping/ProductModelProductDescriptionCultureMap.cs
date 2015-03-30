using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductModelProductDescriptionCultureMap : EntityTypeConfiguration<ProductModelProductDescriptionCulture>
    {
        public ProductModelProductDescriptionCultureMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductModelID, t.ProductDescriptionID, t.CultureID });

            // Properties
            this.Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ProductDescriptionID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CultureID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("ProductModelProductDescriptionCulture", "Production");
            this.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            this.Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            this.Property(t => t.CultureID).HasColumnName("CultureID");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Culture);
            this.HasRequired(t => t.ProductDescription);
            this.HasRequired(t => t.ProductModel)
                .WithMany(t => t.ProductModelProductDescriptionCulture)
                .HasForeignKey(d => d.ProductModelID);

        }
    }
}
