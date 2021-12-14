using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            // Primary Key
            HasKey(t => t.ShoppingCartItemID);

            // Properties
            Property(t => t.ShoppingCartID)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ShoppingCartItem", "Sales");
            Property(t => t.ShoppingCartItemID).HasColumnName("ShoppingCartItemID");
            Property(t => t.ShoppingCartID).HasColumnName("ShoppingCartID");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.DateCreated).HasColumnName("DateCreated");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }

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
            builder.Property(t => t.Quantity).HasColumnName("Quantity");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.DateCreated).HasColumnName("DateCreated");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);
        }
    }
}
