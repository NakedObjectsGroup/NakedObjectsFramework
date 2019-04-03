using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
}
