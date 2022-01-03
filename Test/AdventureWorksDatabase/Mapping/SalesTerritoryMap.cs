using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesTerritory> builder)
        {
            builder.HasKey(t => t.TerritoryID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.mappedCountryRegionCode)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(t => t.mappedGroup)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SalesTerritory", "Sales");
            builder.Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedCountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.mappedGroup).HasColumnName("Group");
            builder.Property(t => t.mappedSalesYTD).HasColumnName("SalesYTD");
            builder.Property(t => t.mappedSalesLastYear).HasColumnName("SalesLastYear");
            builder.Property(t => t.mappedCostYTD).HasColumnName("CostYTD");
            builder.Property(t => t.mappedCostLastYear).HasColumnName("CostLastYear");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
