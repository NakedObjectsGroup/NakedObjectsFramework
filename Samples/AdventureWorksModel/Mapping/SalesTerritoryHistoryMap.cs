using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesTerritoryHistoryMap : EntityTypeConfiguration<SalesTerritoryHistory>
    {
        public SalesTerritoryHistoryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SalesPersonID, t.SalesTerritoryID, t.StartDate });

            // Properties
            this.Property(t => t.SalesPersonID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SalesTerritoryID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("SalesTerritoryHistory", "Sales");
            this.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            this.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.SalesPerson)
                .WithMany(t => t.TerritoryHistory)
                .HasForeignKey(d => d.SalesPersonID);
            this.HasRequired(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

        }
    }
}
