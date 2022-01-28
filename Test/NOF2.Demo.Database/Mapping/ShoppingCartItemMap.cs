using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.HasKey(t => t.ShoppingCartItemID);

            // Properties
            builder.Property(t => t.ShoppingCartID)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ShoppingCartItem", "Sales");
            builder.Property(t => t.ShoppingCartItemID).HasColumnName("ShoppingCartItemID");
            builder.Property(t => t.ShoppingCartID).HasColumnName("ShoppingCartID");
            builder.Property(t => t.mappedQuantity).HasColumnName("Quantity");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.DateCreated).HasColumnName("DateCreated");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
        }
    }
}
