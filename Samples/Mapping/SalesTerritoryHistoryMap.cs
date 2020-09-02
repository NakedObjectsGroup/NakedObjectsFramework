using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

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
}
