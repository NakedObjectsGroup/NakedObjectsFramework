using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesPersonQuotaHistory> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.mappedQuotaDate });

            // Properties
            builder.Property(t => t.BusinessEntityID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesPersonQuotaHistory", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedQuotaDate).HasColumnName("QuotaDate");
            builder.Property(t => t.mappedSalesQuota).HasColumnName("SalesQuota");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.SalesPerson)
                   .WithMany(t => t.mappedQuotaHistory)
                   .HasForeignKey(d => d.BusinessEntityID);
        }
    }
}
