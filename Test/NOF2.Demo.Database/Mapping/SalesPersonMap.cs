using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesPerson> builder)
        {
            builder.HasKey(t => t.BusinessEntityID);

            // Properties
            builder.Property(t => t.BusinessEntityID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesPerson", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.mappedSalesQuota).HasColumnName("SalesQuota");
            builder.Property(t => t.mappedBonus).HasColumnName("Bonus");
            builder.Property(t => t.mappedCommissionPct).HasColumnName("CommissionPct");
            builder.Property(t => t.mappedSalesYTD).HasColumnName("SalesYTD");
            builder.Property(t => t.mappedSalesLastYear).HasColumnName("SalesLastYear");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.EmployeeDetails).WithOne(t => t.SalesPerson).HasForeignKey<Employee>(t => t.BusinessEntityID);
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

            builder.Ignore(t => t.PersonDetails);
        }
    }
}
