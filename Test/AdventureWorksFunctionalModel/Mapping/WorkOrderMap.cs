using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class WorkOrderMap : EntityTypeConfiguration<WorkOrder>
    {
        public WorkOrderMap()
        {
            // Primary Key
            HasKey(t => t.WorkOrderID);

            Ignore(t => t.AnAlwaysHiddenReadOnlyProperty);

            // Properties
            // Table & Column Mappings
            ToTable("WorkOrder", "Production");
            Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.OrderQty).HasColumnName("OrderQty");
            Property(t => t.StockedQty).HasColumnName("StockedQty").
                HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(t => t.ScrappedQty).HasColumnName("ScrappedQty");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.DueDate).HasColumnName("DueDate");
            Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            HasOptional(t => t.ScrapReason).WithMany().HasForeignKey(t => t.ScrapReasonID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<WorkOrder> builder)
        {
            builder.HasKey(t => t.WorkOrderID);

            builder.Ignore(t => t.AnAlwaysHiddenReadOnlyProperty);

            // Properties
            // Table & Column Mappings
            builder.ToTable("WorkOrder", "Production");
            builder.Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.OrderQty).HasColumnName("OrderQty");
            builder.Property(t => t.StockedQty).HasColumnName("StockedQty").
                                        ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.ScrappedQty).HasColumnName("ScrappedQty");
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.DueDate).HasColumnName("DueDate");
            builder.Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            builder.HasOne(t => t.ScrapReason).WithMany().HasForeignKey(t => t.ScrapReasonID);
        }
    }
}
