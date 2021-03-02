using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class CountryRegionCurrencyMap : EntityTypeConfiguration<CountryRegionCurrency>
    {
        public CountryRegionCurrencyMap()
        {
            // Primary Key
            HasKey(t => new { t.CountryRegionCode, t.CurrencyCode });

            // Properties
            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.CurrencyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            ToTable("CountryRegionCurrency", "Sales");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            HasRequired(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyCode); 

        }
    }

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
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            builder.HasOne(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyCode);
        }
    }
}
