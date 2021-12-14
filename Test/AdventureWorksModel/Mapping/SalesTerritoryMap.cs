using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class SalesTerritoryMap : EntityTypeConfiguration<SalesTerritory>
    {
        public SalesTerritoryMap()
        {
            // Primary Key
            HasKey(t => t.TerritoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3);

            Property(t => t.Group)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("SalesTerritory", "Sales");
            Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            Property(t => t.Group).HasColumnName("Group");
            Property(t => t.SalesYTD).HasColumnName("SalesYTD");
            Property(t => t.SalesLastYear).HasColumnName("SalesLastYear");
            Property(t => t.CostYTD).HasColumnName("CostYTD");
            Property(t => t.CostLastYear).HasColumnName("CostLastYear");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesTerritory> builder)
        {
            builder.HasKey(t => t.TerritoryID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.CountryRegionCode)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(t => t.Group)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SalesTerritory", "Sales");
            builder.Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.CountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.Group).HasColumnName("Group");
            builder.Property(t => t.SalesYTD).HasColumnName("SalesYTD");
            builder.Property(t => t.SalesLastYear).HasColumnName("SalesLastYear");
            builder.Property(t => t.CostYTD).HasColumnName("CostYTD");
            builder.Property(t => t.CostLastYear).HasColumnName("CostLastYear");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
