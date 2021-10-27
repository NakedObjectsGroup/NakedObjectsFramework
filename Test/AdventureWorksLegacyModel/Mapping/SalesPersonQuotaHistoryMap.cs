using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class SalesPersonQuotaHistoryMap : EntityTypeConfiguration<SalesPersonQuotaHistory>
    {
        public SalesPersonQuotaHistoryMap()
        {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.QuotaDate });

            // Properties
            Property(t => t.BusinessEntityID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SalesPersonQuotaHistory", "Sales");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.QuotaDate).HasColumnName("QuotaDate");
            Property(t => t.SalesQuota).HasColumnName("SalesQuota");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.SalesPerson)
                .WithMany(t => t.QuotaHistory)
                .HasForeignKey(d => d.BusinessEntityID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesPersonQuotaHistory> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.QuotaDate });

            // Properties
            builder.Property(t => t.BusinessEntityID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesPersonQuotaHistory", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.QuotaDate).HasColumnName("QuotaDate");
            builder.Property(t => t.SalesQuota).HasColumnName("SalesQuota");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.SalesPerson)
                   .WithMany(t => t.QuotaHistory)
                   .HasForeignKey(d => d.BusinessEntityID);
        }
    }
}
