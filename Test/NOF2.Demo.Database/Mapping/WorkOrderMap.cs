using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
  public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<WorkOrder> builder)
        {
            builder.HasKey(t => t.WorkOrderID);

            //builder.Ignore(t => t.AnAlwaysHiddenReadOnlyProperty);

            // Properties
            // Table & Column Mappings
            builder.ToTable("WorkOrder", "Production");
            builder.Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.mappedOrderQty).HasColumnName("OrderQty");
            builder.Property(t => t.mappedStockedQty).HasColumnName("StockedQty").
                    ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.mappedScrappedQty).HasColumnName("ScrappedQty");
            builder.Property(t => t.mappedStartDate).HasColumnName("StartDate");
            builder.Property(t => t.mappedEndDate).HasColumnName("EndDate");
            builder.Property(t => t.mappedDueDate).HasColumnName("DueDate");
            builder.Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            builder.HasOne(t => t.ScrapReason).WithMany().HasForeignKey(t => t.ScrapReasonID);
        }
    }
}
