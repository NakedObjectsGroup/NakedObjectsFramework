using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
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

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<TransactionHistory> builder)
        {
            builder.HasKey(t => t.TransactionID);

            // Properties
            builder.Property(t => t.TransactionType)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(1);

            // Table & Column Mappings
            builder.ToTable("TransactionHistory", "Production");
            builder.Property(t => t.TransactionID).HasColumnName("TransactionID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.ReferenceOrderID).HasColumnName("ReferenceOrderID");
            builder.Property(t => t.ReferenceOrderLineID).HasColumnName("ReferenceOrderLineID");
            builder.Property(t => t.TransactionDate).HasColumnName("TransactionDate");
            builder.Property(t => t.TransactionType).HasColumnName("TransactionType");
            builder.Property(t => t.Quantity).HasColumnName("Quantity");
            builder.Property(t => t.ActualCost).HasColumnName("ActualCost");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
        }
    }
}
