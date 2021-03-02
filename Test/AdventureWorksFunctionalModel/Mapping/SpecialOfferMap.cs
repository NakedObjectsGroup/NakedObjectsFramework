using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class SpecialOfferMap : EntityTypeConfiguration<SpecialOffer>
    {
        public SpecialOfferMap()
        {
            // Primary Key
            HasKey(t => t.SpecialOfferID);

            // Properties
            Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(255);

            Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("SpecialOffer", "Sales");
            Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.DiscountPct).HasColumnName("DiscountPct");
            Property(t => t.Type).HasColumnName("Type");
            Property(t => t.Category).HasColumnName("Category");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.MinQty).HasColumnName("MinQty");
            Property(t => t.MaxQty).HasColumnName("MaxQty");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SpecialOffer> builder)
        {
            builder.HasKey(t => t.SpecialOfferID);

            // Properties
            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SpecialOffer", "Sales");
            builder.Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.DiscountPct).HasColumnName("DiscountPct");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.Category).HasColumnName("Category");
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.MinQty).HasColumnName("MinQty");
            builder.Property(t => t.MaxQty).HasColumnName("MaxQty");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
