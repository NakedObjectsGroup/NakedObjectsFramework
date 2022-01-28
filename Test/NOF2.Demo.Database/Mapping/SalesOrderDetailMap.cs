using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesOrderDetail> builder)
        {
            builder.HasKey(t => new { t.SalesOrderID, t.SalesOrderDetailID });

            //Ignores
            builder.Ignore(t => t.Product);
            builder.Ignore(t => t.SpecialOffer);

            // Properties
            builder.Property(t => t.SalesOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.SalesOrderDetailID)
                   .ValueGeneratedOnAdd();

            builder.Property(t => t.mappedCarrierTrackingNumber)
                   .HasMaxLength(25);

            // Table & Column Mappings
            builder.ToTable("SalesOrderDetail", "Sales");
            builder.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            builder.Property(t => t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID");
            builder.Property(t => t.mappedCarrierTrackingNumber).HasColumnName("CarrierTrackingNumber");
            builder.Property(t => t.mappedOrderQty).HasColumnName("OrderQty");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            builder.Property(t => t.mappedUnitPrice).HasColumnName("UnitPrice");
            builder.Property(t => t.mappedUnitPriceDiscount).HasColumnName("UnitPriceDiscount");
            builder.Property(t => t.mappedLineTotal).HasColumnName("LineTotal").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.SalesOrderHeader)
                   .WithMany(t => t.mappedDetails)
                   .HasForeignKey(d => d.SalesOrderID);
            builder.HasOne(t => t.SpecialOfferProduct).WithMany().HasForeignKey(t => new { t.SpecialOfferID, t.ProductID });
        }
    }
}
