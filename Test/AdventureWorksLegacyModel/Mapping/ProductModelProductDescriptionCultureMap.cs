using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
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

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductModelProductDescriptionCulture> builder)
        {
            builder.HasKey(t => new { t.ProductModelID, t.ProductDescriptionID, t.CultureID });

            // Properties
            builder.Property(t => t.ProductModelID)
                   .ValueGeneratedNever();

            builder.Property(t => t.ProductDescriptionID)
                   .ValueGeneratedNever();

            builder.Property(t => t.CultureID)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(6);

            // Table & Column Mappings
            builder.ToTable("ProductModelProductDescriptionCulture", "Production");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            builder.Property(t => t.CultureID).HasColumnName("CultureID");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.Culture).WithMany().HasForeignKey(t => t.CultureID);
            builder.HasOne(t => t.ProductDescription).WithMany().HasForeignKey(t => t.ProductDescriptionID);
            builder.HasOne(t => t.ProductModel)
                   .WithMany(t => t.ProductModelProductDescriptionCulture)
                   .HasForeignKey(d => d.ProductModelID);
        }
    }
}
