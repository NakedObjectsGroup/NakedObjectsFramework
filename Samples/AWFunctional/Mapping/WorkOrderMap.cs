using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class WorkOrderMap : EntityTypeConfiguration<WorkOrder>
    {
        public WorkOrderMap()
        {
            // Primary Key
            HasKey(t => t.WorkOrderID);

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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
            HasOptional(t => t.ScrapReason).WithMany().HasForeignKey(t => t.ScrapReasonID);

        }
    }
}
