using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class TransactionHistoryMap : EntityTypeConfiguration<TransactionHistory>
    {
        public TransactionHistoryMap()
        {
            // Primary Key
            HasKey(t => t.TransactionID);

            // Properties
            Property(t => t.TransactionType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            ToTable("TransactionHistory", "Production");
            Property(t => t.TransactionID).HasColumnName("TransactionID");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.ReferenceOrderID).HasColumnName("ReferenceOrderID");
            Property(t => t.ReferenceOrderLineID).HasColumnName("ReferenceOrderLineID");
            Property(t => t.TransactionDate).HasColumnName("TransactionDate");
            Property(t => t.TransactionType).HasColumnName("TransactionType");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.ActualCost).HasColumnName("ActualCost");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }
}
