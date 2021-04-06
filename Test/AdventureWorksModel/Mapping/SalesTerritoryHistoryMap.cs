using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class SalesTerritoryHistoryMap : EntityTypeConfiguration<SalesTerritoryHistory>
    {
        public SalesTerritoryHistoryMap()
        {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.SalesTerritoryID, t.StartDate });

            // Properties
            Property(t => t.BusinessEntityID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SalesTerritoryID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SalesTerritoryHistory", "Sales");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.SalesPerson)
                .WithMany(t => t.TerritoryHistory)
                .HasForeignKey(d => d.BusinessEntityID);
            HasRequired(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesTerritoryHistory> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.SalesTerritoryID, t.StartDate });

            // Properties
            builder.Property(t => t.BusinessEntityID)
                   .ValueGeneratedNever();

            builder.Property(t => t.SalesTerritoryID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesTerritoryHistory", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.SalesPerson)
                   .WithMany(t => t.TerritoryHistory)
                   .HasForeignKey(d => d.BusinessEntityID);
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }
}
