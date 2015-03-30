using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            // Primary Key
            this.HasKey(t => t.ShoppingCartItemID);

            // Properties
            this.Property(t => t.ShoppingCartID)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ShoppingCartItem", "Sales");
            this.Property(t => t.ShoppingCartItemID).HasColumnName("ShoppingCartItemID");
            this.Property(t => t.ShoppingCartID).HasColumnName("ShoppingCartID");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ShoppingCartItems)
                .HasForeignKey(d => d.ProductID);

        }
    }
}
