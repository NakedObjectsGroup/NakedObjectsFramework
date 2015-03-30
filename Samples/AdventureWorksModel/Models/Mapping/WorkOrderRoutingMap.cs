using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class WorkOrderRoutingMap : EntityTypeConfiguration<WorkOrderRouting>
    {
        public WorkOrderRoutingMap()
        {
            // Primary Key
            this.HasKey(t => new { t.WorkOrderID, t.ProductID, t.OperationSequence });

            // Properties
            this.Property(t => t.WorkOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OperationSequence)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("WorkOrderRouting", "Production");
            this.Property(t => t.WorkOrderID).HasColumnName("WorkOrderID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.OperationSequence).HasColumnName("OperationSequence");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.ScheduledStartDate).HasColumnName("ScheduledStartDate");
            this.Property(t => t.ScheduledEndDate).HasColumnName("ScheduledEndDate");
            this.Property(t => t.ActualStartDate).HasColumnName("ActualStartDate");
            this.Property(t => t.ActualEndDate).HasColumnName("ActualEndDate");
            this.Property(t => t.ActualResourceHrs).HasColumnName("ActualResourceHrs");
            this.Property(t => t.PlannedCost).HasColumnName("PlannedCost");
            this.Property(t => t.ActualCost).HasColumnName("ActualCost");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Location)
                .WithMany(t => t.WorkOrderRoutings)
                .HasForeignKey(d => d.LocationID);
            this.HasRequired(t => t.WorkOrder)
                .WithMany(t => t.WorkOrderRoutings)
                .HasForeignKey(d => d.WorkOrderID);

        }
    }
}
