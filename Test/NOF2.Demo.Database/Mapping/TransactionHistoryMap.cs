using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<TransactionHistory> builder)
        {
            builder.HasKey(t => t.TransactionID);

            // Properties
            builder.Property(t => t.mappedTransactionType)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(1);

            // Table & Column Mappings
            builder.ToTable("TransactionHistory", "Production");
            builder.Property(t => t.TransactionID).HasColumnName("TransactionID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.ReferenceOrderID).HasColumnName("ReferenceOrderID");
            builder.Property(t => t.ReferenceOrderLineID).HasColumnName("ReferenceOrderLineID");
            builder.Property(t => t.mappedTransactionDate).HasColumnName("TransactionDate");
            builder.Property(t => t.mappedTransactionType).HasColumnName("TransactionType");
            builder.Property(t => t.mappedQuantity).HasColumnName("Quantity");
            builder.Property(t => t.mappedActualCost).HasColumnName("ActualCost");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
        }
    }
}
