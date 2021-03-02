using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class SpecialOfferProductMap : EntityTypeConfiguration<SpecialOfferProduct>
    {
        public SpecialOfferProductMap()
        {
            // Primary Key
            HasKey(t => new { t.SpecialOfferID, t.ProductID });

            // Properties
            Property(t => t.SpecialOfferID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SpecialOfferProduct", "Sales");
            Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.Product)
                .WithMany(t => t.SpecialOfferProduct)
                .HasForeignKey(d => d.ProductID);
            HasRequired(t => t.SpecialOffer).WithMany().HasForeignKey(t => t.SpecialOfferID);

        }
    }

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
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.SpecialOfferProduct)
                   .HasForeignKey(d => d.ProductID);
            builder.HasOne(t => t.SpecialOffer).WithMany().HasForeignKey(t => t.SpecialOfferID);
        }
    }
}
