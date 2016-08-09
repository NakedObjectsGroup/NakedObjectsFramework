using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductModelProductDescriptionCultureMap : EntityTypeConfiguration<ProductModelProductDescriptionCulture>
    {
        public ProductModelProductDescriptionCultureMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductModelID, t.ProductDescriptionID, t.CultureID });

            // Properties
            Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductDescriptionID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.CultureID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            ToTable("ProductModelProductDescriptionCulture", "Production");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            Property(t => t.CultureID).HasColumnName("CultureID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Culture).WithMany().HasForeignKey(t => t.CultureID);
            HasRequired(t => t.ProductDescription).WithMany().HasForeignKey(t => t.ProductDescriptionID);
            HasRequired(t => t.ProductModel)
                .WithMany(t => t.ProductModelProductDescriptionCulture)
                .HasForeignKey(d => d.ProductModelID);

        }
    }
}
