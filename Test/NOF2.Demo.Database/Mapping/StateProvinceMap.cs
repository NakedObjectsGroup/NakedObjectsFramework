using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<StateProvince> builder)
        {
            builder.HasKey(t => t.StateProvinceID);

            // Properties
            builder.Property(t => t.mappedStateProvinceCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            builder.Property(t => t.CountryRegionCode)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("StateProvince", "Person");
            builder.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            builder.Property(t => t.mappedStateProvinceCode).HasColumnName("StateProvinceCode");
            builder.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.mappedIsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.CountryRegion).WithMany().HasForeignKey(t => t.CountryRegionCode);
            builder.HasOne(t => t.SalesTerritory)
                   .WithMany(t => t.mappedStateProvince)
                   .HasForeignKey(d => d.TerritoryID);
        }
    }
}
