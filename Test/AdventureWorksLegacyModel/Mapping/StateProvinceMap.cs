using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class StateProvinceMap : EntityTypeConfiguration<StateProvince>
    {
        public StateProvinceMap()
        {
            // Primary Key
            HasKey(t => t.StateProvinceID);

            // Properties
            Property(t => t.StateProvinceCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("StateProvince", "Person");
            Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            Property(t => t.StateProvinceCode).HasColumnName("StateProvinceCode");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            HasRequired(t => t.SalesTerritory)
                .WithMany(t => t.StateProvince)
                .HasForeignKey(d => d.TerritoryID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<StateProvince> builder)
        {
            builder.HasKey(t => t.StateProvinceID);

            // Properties
            builder.Property(t => t.StateProvinceCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            builder.Property(t => t.CountryRegionCode)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("StateProvince", "Person");
            builder.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            builder.Property(t => t.StateProvinceCode).HasColumnName("StateProvinceCode");
            builder.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            builder.HasOne(t => t.SalesTerritory)
                   .WithMany(t => t.StateProvince)
                   .HasForeignKey(d => d.TerritoryID);
        }
    }
}
