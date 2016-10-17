using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesOrderDetailMap : EntityTypeConfiguration<SalesOrderDetail>
    {
        public SalesOrderDetailMap()
        {
            // Primary Key
            HasKey(t => new { t.SalesOrderID, t.SalesOrderDetailID });

            //Ignores
            Ignore(t => t.Product);

            // Properties
            Property(t => t.SalesOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SalesOrderDetailID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.CarrierTrackingNumber)
                .HasMaxLength(25);

            // Table & Column Mappings
            ToTable("SalesOrderDetail", "Sales");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID");
            Property(t => t.CarrierTrackingNumber).HasColumnName("CarrierTrackingNumber");
            Property(t => t.OrderQty).HasColumnName("OrderQty");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            Property(t => t.UnitPriceDiscount).HasColumnName("UnitPriceDiscount");
            Property(t => t.LineTotal).HasColumnName("LineTotal").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.SalesOrderHeader)
                .WithMany(t => t.Details)
                .HasForeignKey(d => d.SalesOrderID);
            HasRequired(t => t.SpecialOfferProduct).WithMany().HasForeignKey(t => new { t.SpecialOfferID, t.ProductID });

        }
    }
}
