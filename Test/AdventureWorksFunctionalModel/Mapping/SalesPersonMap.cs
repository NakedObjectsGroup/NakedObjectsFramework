using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.EmployeeDetails).WithOptional(t => t.SalesPerson);
            HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

            Ignore(t => t.PersonDetails);

        }
    }

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
            builder.Property(t => t.SalesQuota).HasColumnName("SalesQuota");
            builder.Property(t => t.Bonus).HasColumnName("Bonus");
            builder.Property(t => t.CommissionPct).HasColumnName("CommissionPct");
            builder.Property(t => t.SalesYTD).HasColumnName("SalesYTD");
            builder.Property(t => t.SalesLastYear).HasColumnName("SalesLastYear");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
           //builder.HasRequired(t => t.EmployeeDetails).WithOptional(t => t.SalesPerson);
           // builder.HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

            builder.Ignore(t => t.PersonDetails);
        }
    }
}
