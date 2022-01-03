using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesOrderHeaderSalesReason> builder)
        {
            builder.HasKey(t => new { t.SalesOrderID, t.SalesReasonID });

            // Properties
            builder.Property(t => t.SalesOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.SalesReasonID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("SalesOrderHeaderSalesReason", "Sales");
            builder.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            builder.Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.SalesOrderHeader);
                   //.WithMany(t => t.mappedSalesOrderHeaderSalesReason)
                   //.HasForeignKey(d => d.SalesOrderID);
            builder.HasOne(t => t.SalesReason).WithMany().HasForeignKey(t => t.SalesReasonID);
        }
    }
}
