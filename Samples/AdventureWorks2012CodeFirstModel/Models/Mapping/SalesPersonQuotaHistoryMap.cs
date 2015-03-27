using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class SalesPersonQuotaHistoryMap : EntityTypeConfiguration<SalesPersonQuotaHistory>
    {
        public SalesPersonQuotaHistoryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SalesPersonID, t.QuotaDate });

            // Properties
            this.Property(t => t.SalesPersonID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("SalesPersonQuotaHistory", "Sales");
            this.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            this.Property(t => t.QuotaDate).HasColumnName("QuotaDate");
            this.Property(t => t.SalesQuota).HasColumnName("SalesQuota");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.SalesPerson)
                .WithMany(t => t.SalesPersonQuotaHistories)
                .HasForeignKey(d => d.SalesPersonID);

        }
    }
}
