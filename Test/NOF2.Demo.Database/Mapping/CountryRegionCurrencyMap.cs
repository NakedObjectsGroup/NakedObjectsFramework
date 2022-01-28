using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<CountryRegionCurrency> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.CountryRegionCode, t.CurrencyCode });

            // Properties
            builder.Property(t => t.CountryRegionCode)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(t => t.CurrencyCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            // Table & Column Mappings
            builder.ToTable("CountryRegionCurrency", "Sales");
            builder.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            builder.HasOne(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyCode);
        }
    }
}
