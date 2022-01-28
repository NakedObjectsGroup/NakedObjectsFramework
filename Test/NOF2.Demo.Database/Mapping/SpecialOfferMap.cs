using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SpecialOffer> builder)
        {
            builder.HasKey(t => t.SpecialOfferID);

            //builder.Ignore(t => t.mappedDescription).Ignore(t => t.mappedType).Ignore(t => t.mappedCategory).Ignore(t => t.mappedStartDate).Ignore(t => t.mappedEndDate)
            //    .Ignore(t => t.mappedMinQty).Ignore(t => t.mappedMaxQty).Ignore(t => t.mappedModifiedDate);

            builder.Ignore(t => t.Container);
            // Table & Column Mappings
            builder.ToTable("SpecialOffer", "Sales");
            builder.Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            builder.Property(t => t.mappedDescription).HasColumnName("Description").IsRequired().HasMaxLength(255);
            builder.Property(t => t.mappedDiscountPct).HasColumnName("DiscountPct");
            builder.Property(t => t.mappedType).HasColumnName("Type").IsRequired().HasMaxLength(50);
            builder.Property(t => t.mappedCategory).HasColumnName("Category").IsRequired().HasMaxLength(50); ;
            builder.Property(t => t.mappedStartDate).HasColumnName("StartDate");
            builder.Property(t => t.mappedEndDate).HasColumnName("EndDate");
            builder.Property(t => t.mappedMinQty).HasColumnName("MinQty");
            builder.Property(t => t.mappedMaxQty).HasColumnName("MaxQty");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
