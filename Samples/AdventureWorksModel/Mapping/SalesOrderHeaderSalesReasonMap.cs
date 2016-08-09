using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesOrderHeaderSalesReasonMap : EntityTypeConfiguration<SalesOrderHeaderSalesReason>
    {
        public SalesOrderHeaderSalesReasonMap()
        {
            // Primary Key
            HasKey(t => new { t.SalesOrderID, t.SalesReasonID });

            // Properties
            Property(t => t.SalesOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SalesReasonID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SalesOrderHeaderSalesReason", "Sales");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.SalesOrderHeader)
                .WithMany(t => t.SalesOrderHeaderSalesReason)
                .HasForeignKey(d => d.SalesOrderID);
            HasRequired(t => t.SalesReason).WithMany().HasForeignKey(t => t.SalesReasonID);

        }
    }
}
