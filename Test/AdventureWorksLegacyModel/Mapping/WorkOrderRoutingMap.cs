using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class WorkOrderRoutingMap : EntityTypeConfiguration<WorkOrderRouting>
    {
        public WorkOrderRoutingMap()
        {
            // Primary Key
            HasKey(t => new { t.WorkOrderID, t.ProductID, t.OperationSequence });

            // Properties
            Property(t => t.WorkOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.OperationSequence)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("WorkOrderRouting", "Production");
            Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.OperationSequence).HasColumnName("OperationSequence");
            Property(t => t.LocationID).HasColumnName("LocationID");
            Property(t => t.ScheduledStartDate).HasColumnName("ScheduledStartDate");
            Property(t => t.ScheduledEndDate).HasColumnName("ScheduledEndDate");
            Property(t => t.ActualStartDate).HasColumnName("ActualStartDate");
            Property(t => t.ActualEndDate).HasColumnName("ActualEndDate");
            Property(t => t.ActualResourceHrs).HasColumnName("ActualResourceHrs");
            Property(t => t.PlannedCost).HasColumnName("PlannedCost");
            Property(t => t.ActualCost).HasColumnName("ActualCost");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            HasRequired(t => t.WorkOrder)
                .WithMany(t => t.WorkOrderRoutings)
                .HasForeignKey(d => d.WorkOrderID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<WorkOrderRouting> builder)
        {
            builder.HasKey(t => new { t.WorkOrderID, t.ProductID, t.OperationSequence });

            // Properties
            builder.Property(t => t.WorkOrderID)
                   .ValueGeneratedNever();

            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            builder.Property(t => t.OperationSequence)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("WorkOrderRouting", "Production");
            builder.Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.OperationSequence).HasColumnName("OperationSequence");
            builder.Property(t => t.LocationID).HasColumnName("LocationID");
            builder.Property(t => t.ScheduledStartDate).HasColumnName("ScheduledStartDate");
            builder.Property(t => t.ScheduledEndDate).HasColumnName("ScheduledEndDate");
            builder.Property(t => t.ActualStartDate).HasColumnName("ActualStartDate");
            builder.Property(t => t.ActualEndDate).HasColumnName("ActualEndDate");
            builder.Property(t => t.ActualResourceHrs).HasColumnName("ActualResourceHrs");
            builder.Property(t => t.PlannedCost).HasColumnName("PlannedCost");
            builder.Property(t => t.ActualCost).HasColumnName("ActualCost");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Location).WithMany().HasForeignKey(t => t.LocationID);
            builder.HasOne(t => t.WorkOrder)
                   .WithMany(t => t.WorkOrderRoutings)
                   .HasForeignKey(d => d.WorkOrderID);
        }
    }
}
