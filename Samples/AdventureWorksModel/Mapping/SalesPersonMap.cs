using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesPersonMap : EntityTypeConfiguration<SalesPerson>
    {
        public SalesPersonMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Properties
            Property(t => t.BusinessEntityID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SalesPerson", "Sales");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            Property(t => t.SalesQuota).HasColumnName("SalesQuota");
            Property(t => t.Bonus).HasColumnName("Bonus");
            Property(t => t.CommissionPct).HasColumnName("CommissionPct");
            Property(t => t.SalesYTD).HasColumnName("SalesYTD");
            Property(t => t.SalesLastYear).HasColumnName("SalesLastYear");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.EmployeeDetails).WithOptional(t => t.SalesPerson);
            HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

            Ignore(t => t.PersonDetails);

        }
    }
}
