using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesTerritoryHistory> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.SalesTerritoryID, t.mappedStartDate });

            // Properties
            builder.Property(t => t.BusinessEntityID)
                   .ValueGeneratedNever();

            builder.Property(t => t.SalesTerritoryID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesTerritoryHistory", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.mappedStartDate).HasColumnName("StartDate");
            builder.Property(t => t.mappedEndDate).HasColumnName("EndDate");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.SalesPerson)
                   .WithMany(t => t.mappedTerritoryHistory)
                   .HasForeignKey(d => d.BusinessEntityID);
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }
}
