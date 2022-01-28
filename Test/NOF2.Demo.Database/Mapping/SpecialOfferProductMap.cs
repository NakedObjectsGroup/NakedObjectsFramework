using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SpecialOfferProduct> builder)
        {
            builder.HasKey(t => new { t.SpecialOfferID, t.ProductID });

            // Properties
            builder.Property(t => t.SpecialOfferID)
                   .ValueGeneratedNever();

            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SpecialOfferProduct", "Sales");
            builder.Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.SpecialOfferProduct)
                   .HasForeignKey(d => d.ProductID);
            builder.HasOne(t => t.SpecialOffer).WithMany().HasForeignKey(t => t.SpecialOfferID);
        }
    }
}
