using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class CountryRegionMap : EntityTypeConfiguration<CountryRegion> {
        public CountryRegionMap() {
            // Primary Key
            HasKey(t => t.CountryRegionCode);

            // Properties
            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("CountryRegion", "Person");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<CountryRegion> builder) {
            builder.HasKey(t => t.CountryRegionCode);

            // Properties
            builder.Property(t => t.CountryRegionCode)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("CountryRegion", "Person");
            builder.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }
}