using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<WorkOrderRouting> builder)
        {
            builder.HasKey(t => new { t.WorkOrderID, t.ProductID, t.mappedOperationSequence });

            // Properties
            builder.Property(t => t.WorkOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            builder.Property(t => t.mappedOperationSequence)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("WorkOrderRouting", "Production");
            builder.Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.mappedOperationSequence).HasColumnName("OperationSequence");
            builder.Property(t => t.LocationID).HasColumnName("LocationID");
            builder.Property(t => t.mappedScheduledStartDate).HasColumnName("ScheduledStartDate");
            builder.Property(t => t.mappedScheduledEndDate).HasColumnName("ScheduledEndDate");
            builder.Property(t => t.mappedActualStartDate).HasColumnName("ActualStartDate");
            builder.Property(t => t.mappedActualEndDate).HasColumnName("ActualEndDate");
            builder.Property(t => t.mappedActualResourceHrs).HasColumnName("ActualResourceHrs");
            builder.Property(t => t.mappedPlannedCost).HasColumnName("PlannedCost");
            builder.Property(t => t.mappedActualCost).HasColumnName("ActualCost");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            builder.HasOne(t => t.WorkOrder)
                   .WithMany(t => t.mappedWorkOrderRoutings)
                   .HasForeignKey(d => d.WorkOrderID);
        }
    }
}
